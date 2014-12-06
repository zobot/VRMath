﻿using UnityEngine;
using System.Collections;

public class vector_primitives {

	// Use this for initialization
	GameObject arrow1; 
	GameObject arrow2; 
	GameObject ans_arrow; 
	MeshRenderer arrow_mesh1; 
	MeshRenderer arrow_mesh2; 
	MeshRenderer ans_arrow_mesh; 
	public bool not_done = false; 
	public float iterations = 0.0f; 
	//string arrow1_str,arrow2_str,ans_arrow_str;

	public vector_primitives(GameObject parent)
	{



		arrow1 = GameObject.Instantiate( Resources.LoadAssetAtPath("Assets/Resources/Arrow18.prefab", typeof(GameObject))) as GameObject;
		arrow2 = GameObject.Instantiate( Resources.LoadAssetAtPath("Assets/Resources/Arrow18.prefab", typeof(GameObject))) as GameObject;
		ans_arrow = GameObject.Instantiate( Resources.LoadAssetAtPath("Assets/Resources/Arrow18.prefab", typeof(GameObject))) as GameObject;


		arrow1 = arrow1.transform.GetChild (0).gameObject; 
		arrow2 = arrow2.transform.GetChild (0).gameObject; 
		ans_arrow = ans_arrow.transform.GetChild (0).gameObject; 

		arrow_mesh1 = arrow1.GetComponent<MeshRenderer> (); 
		arrow_mesh2 = arrow2.GetComponent<MeshRenderer> (); 
		ans_arrow_mesh = ans_arrow.GetComponent<MeshRenderer> (); 
		
		arrow1.SetActive (false);
		arrow2.SetActive (false);
		ans_arrow.SetActive (false);


		arrow1.renderer.material.color = Color.cyan;
		arrow2.renderer.material.color = Color.magenta;
		ans_arrow.renderer.material.color = Color.green; 
	
	
	}
	

	public bool scale_vector(float scale, Vector3 vec,float num_frames){

		not_done = true; 

		Quaternion rot = Quaternion.LookRotation (vec, Vector3.up);

		ans_arrow.transform.rotation = rot; 
		arrow1.transform.rotation = rot; 

		arrow1.transform.localScale = new Vector3 (vec.sqrMagnitude, 1.0f, 1.0f); 

		if (iterations < 1.0) {
			ans_arrow.transform.localScale = new Vector3 (scale * vec.sqrMagnitude*iterations, 1.0f, 1.0f);
			iterations += 1/num_frames; 
		 
		}
		else{
			iterations = 0.0f; 
			not_done = false; 
		}

		arrow1.SetActive (true);  
		ans_arrow.SetActive (true); 

	
		if (!not_done) {
			ans_arrow_mesh.transform.localScale = arrow1.transform.localScale; 
			arrow1.SetActive(false); 
			ans_arrow.SetActive(false); 
		}
		return not_done; 
	}

	public bool add_vectors(Vector3 vec1, Vector3 vec2,float num_frames){

		not_done = true; 

		Vector3 ans_vec = vec1 + vec2; 

		Quaternion rot1 = Quaternion.LookRotation(vec1, Vector3.up);
		Quaternion rot2 = Quaternion.LookRotation(vec2, Vector3.up);
		Quaternion rot_ans = Quaternion.LookRotation(ans_vec, Vector3.up); 

		ans_arrow.transform.rotation = rot_ans; 
		arrow1.transform.rotation = rot1; 
		arrow2.transform.rotation = rot2; 



		ans_arrow.transform.localScale = new Vector3 (ans_vec.sqrMagnitude, 1.0f, 1.0f); 
		arrow1.transform.localScale = new Vector3 (vec1.sqrMagnitude, 1.0f, 1.0f); 
		arrow2.transform.localScale = new Vector3 (vec2.sqrMagnitude, 1.0f, 1.0f); 

		ans_arrow.transform.position = arrow1.transform.position + 0.5f * arrow1.transform.localScale; 

		arrow_mesh1.enabled = true; 
		arrow_mesh2.enabled = true; 
		ans_arrow_mesh.enabled = true; 

		if (!not_done) {
			arrow_mesh1.enabled = false; 
			arrow_mesh2.enabled = false; 
			ans_arrow_mesh.enabled = false;
		}
		return true; 
		
	}

	public bool multiply_vectors(Vector3 vec1, Vector3 vec2){

		not_done = true; 
		
		Vector3 ans_vec = vec2; 
		
		Quaternion rot1 = Quaternion.LookRotation(vec1, Vector3.up);
		Quaternion rot2 = Quaternion.LookRotation(vec2, Vector3.up);
		Quaternion rot_ans = Quaternion.LookRotation(ans_vec, Vector3.up); 
		
		ans_arrow.transform.rotation = rot_ans; 
		arrow1.transform.rotation = rot1; 
		arrow2.transform.rotation = rot2; 

		float scale = Vector3.Dot (vec2, vec1.normalized); 


		ans_arrow.transform.localScale = new Vector3 (Mathf.Abs(scale), 1.0f, 1.0f); 
		arrow1.transform.localScale = new Vector3 (vec1.sqrMagnitude, 1.0f, 1.0f); 
		arrow2.transform.localScale = new Vector3 (vec2.sqrMagnitude, 1.0f, 1.0f); 
		
		arrow_mesh1.enabled = true; 
		arrow_mesh2.enabled = true; 
		ans_arrow_mesh.enabled = true; 
		
		if (!not_done) {
			arrow_mesh1.enabled = false; 
			arrow_mesh2.enabled = false; 
			ans_arrow_mesh.enabled = false;
		}
		return true; 
	}
}
