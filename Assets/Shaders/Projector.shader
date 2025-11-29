Shader "Cg projector shader for adding light" {
   Properties {
      _ShadowTex ("Projected Image", 2D) = "white" {}
      _Color ("Tint Color", Color) = (1,1,1,1)
   }
   SubShader {
      Pass {      
         Blend SrcAlpha OneMinusSrcAlpha
         ZWrite Off
         Offset -1, -1

         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         // User-specified properties
         uniform sampler2D _ShadowTex; 
         float4 _Color;
 
         // Projector-specific uniforms
         uniform float4x4 unity_Projector; // transformation matrix 
            // from object space to projector space 
 
          struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posProj : TEXCOORD0;
               // position in projector space
         };
 
         vertexOutput vert(vertexInput input) {
            vertexOutput output;
 
            output.posProj = mul(unity_Projector, input.vertex);
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
 
         float4 frag(vertexOutput input) : COLOR {
			  float4 tex = tex2D(_ShadowTex, input.posProj.xy / input.posProj.w); 
            return tex * _Color;
         }
 
         ENDCG
      }
   }  
   Fallback "Projector/Light"
}