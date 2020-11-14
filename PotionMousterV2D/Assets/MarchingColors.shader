// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/ColorMarching"
{
    Properties
    {
        _Color ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,0,0,1)
        _Density ("Density", Float) = 10
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFragColorMarch
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            fixed4 _Color2;
            float _Density;

            fixed4 SpriteFragColorMarch(v2f IN) : SV_Target
            {
                fixed4 c = IN.color;

                float t = -_Time.y / 4;

                float x = IN.texcoord.x + (t - floor(t));

                if(x > 1.0)
                    x = x - 1.0;

                uint m = x * _Density;

                if(m % 2 == 0)
                    c = _Color2;

                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}