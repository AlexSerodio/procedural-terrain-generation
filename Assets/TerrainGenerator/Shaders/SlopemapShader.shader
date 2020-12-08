Shader "Terrain/Slopemap" {
    Properties {
        _PlaneColour ("Plane Colour", Color) = (0,1,0,1)
        _SlopeColour ("Slope Colour", Color) = (1,1,1,1)
        _SlopeThreshold ("Slope Threshold", Range(0,1)) = .5
        _BlendAmount ("Blend Amount", Range(0,1)) = .5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input {
            float3 worldPos;
            float3 worldNormal;
        };

        half _SlopeThreshold;
        half _BlendAmount;
        fixed4 _PlaneColour;
        fixed4 _SlopeColour;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            float slope = 1-IN.worldNormal.y; // slope = 0 when terrain is completely flat
            float blendHeight = _SlopeThreshold * (1-_BlendAmount);
            float weight = 1-saturate((slope-blendHeight)/(_SlopeThreshold-blendHeight));
            o.Albedo = _PlaneColour * weight + _SlopeColour * (1-weight);
        }
        ENDCG
    }
}
