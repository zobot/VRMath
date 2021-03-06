// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
		public class BarAnimation
		{
			GameObject bar; 
			float mag = 0.0f; 
			float scale_const = 0.5f; 

			public BarAnimation (GameObject parent, Color color)
			{
			    bar = GameObject.CreatePrimitive (PrimitiveType.Cube);
				bar.transform.parent = parent.transform; 
				bar.SetActive(false); 

				
				bar.renderer.material.color = color; 
			}

			public void drawVector(Vector3 vec){
				
				Quaternion rot = getRotation (vec); 
				
				Vector3 orign = new Vector3 (-0.5f*vec.magnitude, 0.0f, 0.0f);
				bar.transform.localRotation = rot; 
				bar.transform.localScale = new Vector3 (vec.magnitude, 1.0f, 1.0f); 
				bar.transform.localPosition = rot*orign; 
				
				bar.SetActive(true); 
				mag = vec.magnitude; 
				
			}

			Quaternion getRotation(Vector3 vec){
				//vec comes in as x axis need to derive z and y 
				Vector3 z_axis, y_axis; 
				
				
				y_axis = Quaternion.Euler (new Vector3 (0.0f, 90.0f, 0.0f)) * vec; 
				z_axis = Vector3.Cross (y_axis, vec); 
				
				Quaternion rot =  Quaternion.LookRotation(vec)*Quaternion.Euler (new Vector3 (0.0f, 90.0f, 0.0f));
				
				return rot; 
			}

			public void hideBar(){
				bar.SetActive(false); 
			}
		}
}

