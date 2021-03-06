﻿using UnityEngine;
using System.Collections;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System;

public class PlotManager : MonoBehaviour {
	GameObject meshTopLevelRadial;
	GameObject meshTopLevelSquare;
	OptimizationPlot optPlot;

	public bool displayRadial = true;
	public bool displaySquare = false;
	public bool displayOpt = false;

	bool currentDisplayRadial;
	bool currentDisplaySquare;
	bool currentDisplayOpt;

	GameObject[] squareMeshes;
	GameObject[] radialMeshes;

	public Matrix4x4 QuadForm;
	public Matrix4x4 EllipseTransformer;

	public EigenvalueDecomposition eigen; 
	public Matrix eigenVectors;
	public ComplexVector eigenValues;
	public Matrix ellipseTransformer2dim;
	public Matrix eigenValuesMatInvSquareRoot;
	public Matrix eigenValuesMatSquareRoot;
	public float RadiusScale;
	public double alphaForParabolaTrackingOptimization = 1E-6;
	public double deltaForParabolaTrackingOptimization = 1E-16;
	public Matrix eigenValueOptimizationParabolaTracking;
	public int optimizationTrackingIterationCount = 100;
	
	public Matrix quadForm2dim;
	public Matrix rotationMatrix;
	public Matrix eigsMatrix;
	public Matrix rotationMatrixTranspose;

	Vector3[] currentFingerPoses;
	Matrix[] currentFingerPoses2dim;
	Matrix[] currentFingerPoses2dimRotated;
	

	float diagComponent0;
	float diagComponent2;
	float offDiagComponent;

	double mag0 = 1.0;
	double mag1 = 1.0;


	//Matrix SDPCurrentMatrix;
	Matrix solveFingerMatrix;
	Matrix solveFingerVector;
	Matrix leastSquaresMatrixA;
	Matrix leastSquaresTargetb;
	Matrix nonNegQuadProgMatrixQ;
	Matrix nonNegQuadProgVectorh;
	Vector2 currentFingerHeight;

	public Vector3[] finger_poses;
	public Vector3 optStartPos;

	float currentRotation = 0.0f;

	GameObject controlCube;
	ScaleObject so; 


	public float testFloat = 0.4f;

	int numSquare = 8;
	int numRadial = 2;


	// Use this for initialization
	void Start () {

		// generate meshes
		radialMeshes = new GameObject[numRadial];
		meshTopLevelRadial = new GameObject ("radialGrid250");
		meshTopLevelRadial.transform.parent = gameObject.transform;
		for (int i=0; i<numRadial; i++) {
			GameObject newMeshi = AddMeshComponentChild ("radialGrid250", "radialGrid250_" + i.ToString(), meshTopLevelRadial);
			radialMeshes[i] = newMeshi;
		}

		meshTopLevelRadial.SetActive (displayRadial);
		currentDisplayRadial = displayRadial;

		squareMeshes = new GameObject[numSquare];
		meshTopLevelSquare = new GameObject ("squareGrid500");
		meshTopLevelSquare.transform.parent = gameObject.transform;
		for (int i=0; i<numSquare; i++) {
			GameObject newMeshi = AddMeshComponentChild ("squareGrid500", "squareGrid500_" + i.ToString(), meshTopLevelSquare);
			squareMeshes[i] = newMeshi;
		}

		meshTopLevelSquare.SetActive (displaySquare);
		currentDisplaySquare = displaySquare;

		//gameObject.AddComponent ("OptimizationPlot");
		optPlot = gameObject.GetComponent<OptimizationPlot>(); 


		//generate matrix
		QuadForm = new Matrix4x4();
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				QuadForm [i, j] = 0.0f;
			}
		}
		
		EllipseTransformer = new Matrix4x4();
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				EllipseTransformer [i, j] = 0.0f;
			}
		}
		quadForm2dim = new Matrix(new double[][] {
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0}});
		eigsMatrix = new Matrix(new double[][] {
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0}});
		ellipseTransformer2dim = new Matrix(new double[][] {
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0}});
		eigenValuesMatInvSquareRoot = new Matrix(new double[][] {
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0}});

		eigenValuesMatSquareRoot = new Matrix(new double[][] {
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0}});

		solveFingerMatrix = new Matrix(new double[][] {
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0}});

		leastSquaresMatrixA = new Matrix(new double[][] {
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0}});

		nonNegQuadProgMatrixQ = new Matrix(new double[][] {
			new double[] {0.0, 0.0},
			new double[] {0.0, 0.0}});

		leastSquaresTargetb = new Matrix(new double[][] {
			new double[] {0.0},
			new double[] {0.0},
			new double[] {0.0}});

		nonNegQuadProgVectorh = new Matrix(new double[][] {
			new double[] {0.0},
			new double[] {0.0}});

		solveFingerVector = new Matrix(new double[][] {
			new double[] {0.0},
			new double[] {0.0}});

		eigenValueOptimizationParabolaTracking = new Matrix(new double[][] {
			new double[] {1.0},
			new double[] {1.0}});

		currentFingerPoses2dim = new Matrix[2];
		currentFingerPoses2dim[0] = new Matrix(new double[][] {
			new double[] {0.0},
			new double[] {0.0}});
		currentFingerPoses2dim[1] = new Matrix(new double[][] {
			new double[] {0.0},
			new double[] {0.0}});

		currentFingerPoses2dimRotated = new Matrix[2];
		currentFingerPoses2dimRotated[0] = new Matrix(new double[][] {
			new double[] {0.0},
			new double[] {0.0}});
		currentFingerPoses2dimRotated[1] = new Matrix(new double[][] {
			new double[] {0.0},
			new double[] {0.0}});

		rotationMatrix = new Matrix(new double[][] {
			new double[] {1.0, 0.0},
			new double[] {0.0, 1.0}});

		rotationMatrixTranspose = new Matrix(new double[][] {
			new double[] {1.0, 0.0},
			new double[] {0.0, 1.0}});

		currentFingerHeight = new Vector2(1.0f, 2.0f);

		//SDPCurrentMatrix = new Matrix(new double[][] {
		//	new double[] {1.0, 0.0},
		//	new double[] {0.0, 1.0}});
		
		controlCube = GameObject.Find ("ControlCube");
		so = controlCube.GetComponent<ScaleObject> (); 

		diagComponent0 = (float) 1.0f;
		diagComponent2 = (float) 2.0f;
		offDiagComponent = 0.0f;

		
		QuadForm [0, 0] = diagComponent0;
		QuadForm [2, 2] = diagComponent2;
		QuadForm [0, 2] = offDiagComponent;
		QuadForm [2, 0] = offDiagComponent;

		RadiusScale = 1.0f;



	}
	
	// Update is called once per frame
	void Update () {

		// figuring out plotting
		if (displayRadial != currentDisplayRadial) {
			meshTopLevelRadial.SetActive (displayRadial);
			currentDisplayRadial = displayRadial;
		}
		if (displaySquare != currentDisplaySquare) {
			meshTopLevelSquare.SetActive (displaySquare);
			currentDisplaySquare = displaySquare;
		}
		if (displayOpt != currentDisplayOpt) {
			optPlot.display = displayOpt;
			currentDisplayOpt = displayOpt;
		}

		// Starting matrix calculation

		float t = Time.timeSinceLevelLoad;

		Vector3 scale = so.graph_scale;

		finger_poses = so.finger_poses;
		//Vector3[] finger_poses = new Vector3[2];
		//finger_poses [0] = new Vector3{1.0f, 1.0f, 1.0f};
		//finger_poses [1] = new Vector3{0.5f, 1.0f, 1.0f};

		bool updatedFingerPos = false;
			
		if (finger_poses.Length != 0) {
			if (finger_poses.Length == 2){
				updatedFingerPos = true;
				currentFingerPoses = finger_poses;

				currentFingerPoses2dim[0][0, 0] = currentFingerPoses [0].x;
				currentFingerPoses2dim[0][1, 0] = currentFingerPoses [0].z;
				currentFingerHeight[0] = currentFingerPoses [0].y;
				
				currentFingerPoses2dim[1][0, 0] = currentFingerPoses [1].x;
				currentFingerPoses2dim[1][1, 0] = currentFingerPoses [1].z;
				currentFingerHeight[1] = currentFingerPoses [1].y;

				
				currentFingerPoses2dimRotated[0] = rotationMatrix * currentFingerPoses2dim[0];
				currentFingerPoses2dimRotated[1] = rotationMatrix * currentFingerPoses2dim[1];
			}

			else if (finger_poses.Length == 1){
				//float currentRotation = so.objectRotation
				UnityEngine.Quaternion standardRotation = new UnityEngine.Quaternion(1.0f, 0f, 0f, 0f);
				currentRotation = 2f * Mathf.PI / 180f * UnityEngine.Quaternion.Angle(so.objectRotation, standardRotation);
				float cosRot = Mathf.Cos(currentRotation);
				float sinRot = Mathf.Sin(currentRotation);
				rotationMatrix[0, 0] = cosRot;
				rotationMatrix[1, 0] = sinRot;
				rotationMatrix[0, 1] = -sinRot;
				rotationMatrix[1, 1] = cosRot;
				rotationMatrixTranspose = rotationMatrix.Clone();
				rotationMatrixTranspose.Transpose();
			}


			double mag0xx = (double) ((currentFingerPoses2dimRotated[0][0, 0]) * (currentFingerPoses2dimRotated[0][0, 0]));
			double mag0zz = (double) ((currentFingerPoses2dimRotated[0][1, 0]) * (currentFingerPoses2dimRotated[0][1, 0]));
			double mag1xx = (double) ((currentFingerPoses2dimRotated[1][0, 0]) * (currentFingerPoses2dimRotated[1][0, 0]));
			double mag1zz = (double) ((currentFingerPoses2dimRotated[1][1, 0]) * (currentFingerPoses2dimRotated[1][1, 0]));

			mag0 = Math.Sqrt(mag0xx + mag0zz);
			mag1 = Math.Sqrt(mag1xx + mag1zz);

			double maxHeightTimesTwo0 = (double) Mathf.Max(2 * currentFingerHeight [0], 0.0f);
			double maxHeightTimesTwo1 = (double) Mathf.Max(2 * currentFingerHeight [1], 0.0f);


			// set up linsolve -- note this is bad!!!! we're not taking into account the PSD constraint of the eigenvalues, so our prediction is actually quite bad and not at all what we want
			/*
			solveFingerMatrix [0, 0] = mag0xx;
			solveFingerMatrix [0, 1] = mag0zz;
			solveFingerMatrix [1, 0] = mag1xx;
			solveFingerMatrix [1, 1] = mag1zz;
			
			solveFingerVector [0, 0] = absHeightTimesTwo0;
			solveFingerVector [1, 0] = absHeightTimesTwo1;
			
			Matrix solved = solveFingerMatrix.Solve (solveFingerVector);

			//  Grabby interaction 
			diagComponent0 = (float) solved[0, 0];
			diagComponent2 = (float) solved[1, 0];
			offDiagComponent = 0.0f;
			//RadiusScale = 4.0f * Mathf.Max(finger_poses[0].y, finger_poses[1].y);
			RadiusScale = Mathf.Max(diagComponent0, diagComponent2, 1.0f) * Mathf.Max ((float) mag0, (float) mag1);

			*/


			// NNQP problem see http://arxiv.org/pdf/1406.1008v1.pdf
			// NOTE: for this case, since we use the absolute height |b| and from the nature of our A matrix is that it is nonnegative, we get
			// we know that |Q| = Q = Q+ and h = A^T b = |h| = h+
			// we use this to not compute the negative versions

			// note that this is equivalent to doing ISRA, Image Space Reconstruction Algorithm, as a multiplicative update 
			// (where we also use the the added delta term from the reference)
		
		    leastSquaresMatrixA [0, 0] = mag0xx;
		    leastSquaresMatrixA [0, 1] = mag0zz;
		    leastSquaresMatrixA [1, 0] = mag1xx;
		    leastSquaresMatrixA [1, 1] = mag1zz;
			leastSquaresMatrixA [2, 0] = alphaForParabolaTrackingOptimization;
			leastSquaresMatrixA [2, 1] = alphaForParabolaTrackingOptimization;

			leastSquaresTargetb [0, 0] = maxHeightTimesTwo0;
			leastSquaresTargetb [1, 0] = maxHeightTimesTwo1;
			leastSquaresTargetb [2, 0] = 0.0;


			Matrix leastSquaresMatrixATranspose = leastSquaresMatrixA.Clone();
			leastSquaresMatrixATranspose.Transpose();

			nonNegQuadProgMatrixQ = leastSquaresMatrixATranspose * leastSquaresMatrixA;
			nonNegQuadProgVectorh = leastSquaresMatrixATranspose * leastSquaresTargetb;

			Matrix nonNegQuadProgMatrixQx;

			eigenValueOptimizationParabolaTracking = new Matrix(new double[][] {
				new double[] {1.0},
				new double[] {1.0}});

			// TODO: add change checking so that we dont waste time here if we dont need to.  \
			// Also we could use previous setting for eigenvalues as a warm start, but if one eigenvalue is 0, multiplicative no longer works so would have to restart.
			// Mathematically there's no problem since lambda > 0 but with floats it might be 0 after a finite time.
			for (int i = 0; i < optimizationTrackingIterationCount; i++) {
				nonNegQuadProgMatrixQx = nonNegQuadProgMatrixQ * eigenValueOptimizationParabolaTracking;
				for (int j = 0; j < 2; j++){
					eigenValueOptimizationParabolaTracking[j, 0] = eigenValueOptimizationParabolaTracking[j, 0] * (
																   (nonNegQuadProgVectorh [j, 0] + deltaForParabolaTrackingOptimization) /
																   (nonNegQuadProgMatrixQx[j, 0] + deltaForParabolaTrackingOptimization));
				}
			}


			// TODO: SDP problem to solve for the parabola (see if it feels better than rotation and NNQP


		}

		/*   Grabby interaction */
		diagComponent0 = (float) eigenValueOptimizationParabolaTracking[0, 0];
		diagComponent2 = (float) eigenValueOptimizationParabolaTracking[1, 0];
		offDiagComponent = 0.0f;
		//RadiusScale = 4.0f * Mathf.Max(finger_poses[0].y, finger_poses[1].y);
		//RadiusScale = Mathf.Max(diagComponent0, diagComponent2, 1.0f) * Mathf.Max ((float) mag0, (float) mag1);
		//RadiusScale = 3.0f;



		/* grabby interaction old 
		float diagComponent0 = 2.0f / Mathf.Abs(scale[0]);
		float diagComponent2 = 2.0f / Mathf.Abs(scale[2]);
		float offDiagComponent = 0.0f;
		*/
		
		/*   Sinusoidal watching 
		float diagComponent0 = Mathf.Sin (t) + 1.5f;
		float diagComponent2 = Mathf.Cos (t) + 1.5f;
		float offDiagComponent = 0.5f * Mathf.Sin (1.8f * t) + 0.2f;
		*/
		

		//renderer.material.SetMatrix ("_QuadForm", QuadForm);
		
		// construct matrix by pre multiplying by rotation transpose and post multiplying by rotation
		eigsMatrix [0, 0] = (double) diagComponent0;
		eigsMatrix [1, 0] = (double) offDiagComponent;
		eigsMatrix [0, 1] = (double) offDiagComponent;
		eigsMatrix [1, 1] = (double) diagComponent2;

		quadForm2dim = rotationMatrixTranspose * eigsMatrix * rotationMatrix;

		// Get ellipse transformation
		eigen = quadForm2dim.EigenvalueDecomposition;
		eigenVectors = eigen.EigenVectors;
		Matrix eigenVectorsTranspose = eigenVectors.Clone ();
		eigenVectorsTranspose.Transpose ();
		eigenValues = eigen.EigenValues;

		// identify which eigenvalue goes to x and z



		// project to PSD
		Matrix eigenValuesPSD = new Matrix(new double[][] {
			new double[] {Math.Max(eigenValues[0].Real, 0.0), 0.0},
			new double[] {0.0, Math.Max(eigenValues[1].Real, 0.0)}});
		//eigenValues[0, 0] = Math.Max(eigenValues[0, 0].Modulus, 0.0);
		//eigenValues[1, 1] = Math.Max(eigenValues[1, 1].Modulus, 0.0);


		quadForm2dim = eigenVectors * eigenValuesPSD * eigenVectorsTranspose; // unnecessary if quadform is already PSD, which it should be from our opt


		QuadForm [0, 0] = (float) quadForm2dim[0, 0];
		QuadForm [2, 2] = (float) quadForm2dim[1, 1];
		QuadForm [2, 0] = (float) quadForm2dim[1, 0];
		QuadForm [0, 2] = (float) quadForm2dim[0, 1];

		
		eigenValuesMatInvSquareRoot [0, 0] = 1.0 / Math.Sqrt(eigenValuesPSD [0, 0] + 0.000000001);
		eigenValuesMatInvSquareRoot [1, 1] = 1.0 / Math.Sqrt(eigenValuesPSD [1, 1] + 0.000000001);
		
		ellipseTransformer2dim = eigenVectors * eigenValuesMatInvSquareRoot;
		EllipseTransformer [0, 0] = (float) ellipseTransformer2dim [0, 0];
		EllipseTransformer [1, 1] = 1.0f;
		EllipseTransformer [2, 0] = (float) ellipseTransformer2dim [1, 0];
		EllipseTransformer [0, 2] = (float) ellipseTransformer2dim [0, 1];
		EllipseTransformer [2, 2] = (float) ellipseTransformer2dim [1, 1];
		EllipseTransformer [3, 3] = 1.0f;


		// Calculate right RadiusScale
		if (updatedFingerPos)
		{
			double radiusScaleIterative = 0.0;

			eigenValuesMatSquareRoot [0, 0] = Math.Sqrt(eigenValuesPSD [0, 0]);
			eigenValuesMatSquareRoot [1, 1] = Math.Sqrt(eigenValuesPSD [1, 1]);
			Matrix ellipseTransformer2dimInv = eigenValuesMatSquareRoot * eigenVectorsTranspose;
			for (int i = 0; i < 2; i++){
				double currentNorm = (ellipseTransformer2dimInv * currentFingerPoses2dim[i]).Norm2();
				//double currentNorm = (currentFingerPoses2dim[i]).Norm2();
				radiusScaleIterative = Math.Max (currentNorm, radiusScaleIterative);
			}

			RadiusScale = (float) radiusScaleIterative;
		}
		//RadiusScale = Mathf.Sqrt(5.0f / 2.0f);



		// double check that the matrix solves for the right height 

		if (currentFingerPoses != null) {
			float[] height = new float[2];
			float[] quadHeight = new float[2];
			float[] differenceHeight = new float[2];
			Vector4 currentPoint = new Vector4();
			for (int i = 0; i < 2; i++) {
				height[i] = currentFingerPoses[i].y;
				currentPoint[0] = currentFingerPoses[i].x;
				currentPoint[2] = currentFingerPoses[i].z;
				currentPoint[3] = 1.0f;
				quadHeight[i] = 0.5f * Vector4.Dot(currentPoint, (QuadForm * currentPoint));
				differenceHeight[i] = quadHeight[i] - height[i];
				//print (differenceHeight[i]);
				//if (differenceHeight[i] > 0.5f){
				//	print ("Bad News!  Super bad prediction!");
				//}
			}
			//print (differenceHeight);
		}
	
	}

	GameObject AddMeshComponentChild(string assetPath, string assetName, GameObject parent){
		GameObject newMesh = new GameObject (assetName);
		newMesh.transform.parent = parent.transform;
		newMesh.AddComponent<MeshFilter> ();
		newMesh.AddComponent<MeshRenderer> ();
		//string shaderPre = ShaderGen.shaderPreString();
		//print (shaderPre);
		//newMesh.renderer.material = new Material (shaderPre);
		newMesh.renderer.material = new Material (Shader.Find ("Cg shader for plotting 2d functions"));
		MeshFilter meshFilter = newMesh.GetComponent<MeshFilter> ();
		meshFilter.mesh = Resources.Load<Mesh> (assetPath + "/" + assetName);
		newMesh.AddComponent ("GraphSetter");
		return newMesh;
	}
}
