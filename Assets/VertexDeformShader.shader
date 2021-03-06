﻿Shader "Cg shader for plotting 2d functions" {
    Properties {

    }
   SubShader {
      Pass {   
         Cull Off
         
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         
         
 
         // Uniforms set by a script
         uniform float4x4 _QuadForm; // matrix for quadratic form that determines shape of function
         uniform float4x4 _EllipseTransformer;
         uniform float _RadiusScale;
 
         struct vertexInput {
            float4 vertex : POSITION;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 col : COLOR;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
            
            float4 blendedVertex = _RadiusScale * mul(_EllipseTransformer, input.vertex);      
            blendedVertex[3] = 1.0;  //since we scaled by _RadiusScale including the homogenous part
            
            float height = 0.5 * mul(blendedVertex, mul(_QuadForm, blendedVertex));
            blendedVertex.y = height;
            
            float integralHeight;
            float remainderHeight = modf(10 * abs(height), integralHeight);
             
            output.pos = mul(UNITY_MATRIX_MVP, blendedVertex);
            
 
            output.col = float4(remainderHeight, 1.0 - remainderHeight, height / 0.5, 0.7); 
               // visualize weight0 as red and weight1 as green
            return output;
         }
 		 
         float4 frag(vertexOutput input) : COLOR
         {
            return input.col;
         }
         
 
         ENDCG
      }
   }
}