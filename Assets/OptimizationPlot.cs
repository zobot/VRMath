﻿using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics;
using System;

public class OptimizationPlot : MonoBehaviour {
	
	public enum FunctionOption {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple,
		QuadraticForm
	}

	private delegate float FunctionDelegate (Vector3 p, float t);
	private static FunctionDelegate[] functionDelegates = {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple,
		QuadraticForm
	};
	
	public FunctionOption function = FunctionOption.QuadraticForm;

	public enum DescentOption {
		GradientDescent,
		NewtonsMethod,
		BacktrackingGradientDescent
	}
	
	private delegate Matrix DescentDelegate (Matrix m, float t);
	private static FunctionDelegate[] descentDelegates = {
		//GradientDescent,
		//NewtonsMethod,
		//BacktrackingGradientDescent
	};
	
	public DescentOption descent;// = DescentOption.GradientDescent;
	
	[Range(0.05f, 5.0f)]
	public float learningRate = 0.3f;
	
	[Range(10, 500)]
	public int resolution = 50;
	
	[Range(1, 100)]
	public int iterationCount = 12;
	
	[Range(-0.5f, 0.5f)]
	public float xStart = -0.1f;
	
	[Range(-0.5f, 0.5f)]
	public float zStart = -0.14f;

	public Matrix currentPoint;
	public float currentFuncValue = 0.0f;

	
	private int currentResolution;
	private ParticleSystem.Particle[] optimizationPoints;
	private LineRenderer optimizationRenderer;

	Matrix quadForm2dim;
	GameObject controlCube; 
	ScaleObject so;
	PlotManager plotManagerScript;

	public bool display = false;

	public Vector2 optStartPos;
	public Matrix optStartPosMat;
	
	private void CreateOptimizationPoints () {
		currentResolution = resolution;

		float increment = 1f / (resolution - 1);
		float logInc = Mathf.Log (increment);
		int i = 0;
		/*
		optimizationPoints = new ParticleSystem.Particle[resolution];
		for (int t = 0; t < resolution; t++) {
			Vector3 p = new Vector3(0f, 0f, 0f);
			optimizationPoints[i].position = p;
			optimizationPoints[i].color = new Color(increment * resolution / 2, increment * resolution / 2, increment * resolution / 2);
			optimizationPoints[i++].size = 0.05f;
		}
		*/
		optimizationRenderer.SetVertexCount (iterationCount + 1);
		
	}

	void Start () {
		optimizationRenderer = gameObject.AddComponent<LineRenderer>();
		//optimizationRenderer = new LineRenderer();
		optimizationRenderer.SetWidth (0.10f, 0.10f);
		optimizationRenderer.SetColors (Color.blue, Color.red);
		optimizationRenderer.material.shader = Shader.Find ("Particles/Alpha Blended");
		//print (optimizationRenderer.material);
		//Material (Shader.Find (""));
		//Material material = Material(Shader.Find ("Particles/Alpha Blended"));
		//print (material);
		controlCube = GameObject.Find ("ControlCube");
		so = controlCube.GetComponent<ScaleObject> (); 

		currentPoint = new Matrix(new double[][] {
			new double[] {xStart},
			new double[] {zStart}});

		plotManagerScript = gameObject.GetComponent("PlotManager") as PlotManager;

		optStartPos = new Vector2(1f, .5f);
		optStartPosMat = new Matrix(new double[][] {
			new double[] {1.0},
			new double[] {0.5}});

	}

	
	void Update () {
		if (plotManagerScript.displayOpt != display){
			display = plotManagerScript.displayOpt;
		}
		if (display) {
			optimizationRenderer.enabled = true;
			if (currentResolution != resolution || optimizationPoints == null) {
				CreateOptimizationPoints();
			}
			FunctionDelegate f = functionDelegates[(int)function];
			float t = Time.timeSinceLevelLoad;

			//optimization steps

			Vector3 scale = so.graph_scale;

			//xStart = -0.5f * Mathf.Sin (t);
			//zStart = -0.5f * Mathf.Cos (0.7f * t);
			
			/*   Grabby interaction */
			Matrix a = plotManagerScript.quadForm2dim;
			//Matrix a = QuadraticFormMatrix(t);

			optStartPos = so.optStartPos;
			optStartPosMat[0, 0] = (double) optStartPos.x;
			optStartPosMat[1, 0] = (double) optStartPos.y;
			learningRate = 0.05f * Mathf.Pow(0.75f * so.handDifferenceLearningRate.magnitude, 2);
			
			currentPoint.GetArray () [0] [0] = optStartPos[0];
			currentPoint.GetArray () [1] [0] = optStartPos[1];
			Matrix currentGradient;
			Matrix currentHessianInv;
			Matrix lastPoint = currentPoint.Clone();
			
			//double[] ts = new double[iterationCount + 1];
			//double[] xs = new double[iterationCount + 1];
			//double[] zs = new double[iterationCount + 1];
			//xs[0] = currentPoint.GetArray()[0][0];
			//zs[0] = currentPoint.GetArray()[1][0];
			//optimizatizationVertices 

			Vector3 curVertex = new Vector3();
			curVertex.x = (float) currentPoint.GetArray()[0][0];
			curVertex.z = (float) currentPoint.GetArray()[1][0];
			//curVertex.y = f(curVertex, t);

			Matrix currentPointTranspose = currentPoint.Clone ();
			currentPointTranspose.Transpose ();
			currentFuncValue =  0.5f * (float) (currentPointTranspose * a * currentPoint).GetArray () [0] [0];
			curVertex.y = currentFuncValue + 0.0001f;
			optimizationRenderer.SetPosition (0, curVertex);
			
			for (int i = 0; i < iterationCount; i++) {
				//ts[i] = i;
				currentGradient = a * lastPoint;
				//currentHessianInv = a.Inverse();
				//currentPoint = lastPoint - ((double) learningRate) * currentHessianInv * currentGradient;
				currentPoint = lastPoint - ((double) learningRate) * currentGradient;
				//xs[i + 1] = currentPoint.GetArray()[0][0];
				//zs[i + 1] = currentPoint.GetArray()[1][0];

				curVertex.x = (float) currentPoint.GetArray()[0][0];
				curVertex.z = (float) currentPoint.GetArray()[1][0];

				//float height = f(curVertex, t) + 0.1f;

				currentPointTranspose = currentPoint.Clone ();
				currentPointTranspose.Transpose ();
				currentFuncValue =  0.5f * (float) (currentPointTranspose * a * currentPoint).GetArray () [0] [0];
				curVertex.y = currentFuncValue + 0.0000000001f;

				optimizationRenderer.SetPosition(i + 1, curVertex);
				lastPoint = currentPoint;
			}

			//ts[iterationCount] = iterationCount;

			/*
								*****particle system method******
			IInterpolationMethod xInterp = Interpolation.Create(ts, xs);
			IInterpolationMethod zInterp = Interpolation.Create(ts, zs);
			
			float increment = ((float) iterationCount) / ((float) (resolution - 1));
			
			for (int i = 0; i < resolution; i++) {
				float curInc = increment * i;
				Vector3 p = new Vector3((float) xInterp.Interpolate(curInc), 0.0f, (float) zInterp.Interpolate(curInc));
				p.y = f(p, t); 
				optimizationPoints[i].position = p;
				Color c = optimizationPoints [i].color;
				c.g = ((float) i) / ((float) resolution);
				c.b = ((float) curInc) / ((float) iterationCount);
				//c.g = 1.0f / Mathf.Exp((float) i / 15.0f);
				optimizationPoints[i].color = c;
			}
			
			particleSystem.SetParticles(optimizationPoints, optimizationPoints.Length);
			*/

			/*
									****** line renderer method ******
		*/
		}
		else {
			optimizationRenderer.enabled = false;
		}

	}
	
	private static float Linear (Vector3 p, float t) {
		return p.x;
	}
	
	private static float Exponential (Vector3 p, float t) {
		return p.x * p.x;
	}
	
	private static float Parabola (Vector3 p, float t){
		return 1f - p.x * p.x * p.z * p.z;
	}
	
	private static Matrix QuadraticFormMatrix(float t){
		double xx = (double) Mathf.Sin (t) + 1.5f;
		double zz = (double) Mathf.Cos (t) + 1.5f;
		double xz = (double) 0.5f * Mathf.Sin (1.8f * t) + 0.2f;
		Matrix a = new Matrix (new double[][] {
			new double[] { xx, xz },
			new double[] { xz, zz } });
		return a;
	}
	
	private static float QuadraticForm (Vector3 p, float t){
		Matrix a = QuadraticFormMatrix (t);
		Matrix v = new Matrix(new double[][] {
			new double[] {p.x},
			new double[] {p.z}});
		Matrix vt = v.Clone();
		vt.Transpose();
		Matrix v3 = 0.5 * vt * a * v;
		return (float) v3.GetArray()[0][0];
	}
	
	private static float Sine (Vector3 p, float t){
		return 0.50f +
			0.25f * Mathf.Sin(4 * Mathf.PI * p.x + 4 * t) * Mathf.Sin(2 * Mathf.PI * p.z + t) +
				0.10f * Mathf.Cos(3 * Mathf.PI * p.x + 5 * t) * Mathf.Cos(5 * Mathf.PI * p.z + 3 * t) +
				0.15f * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
	}
	
	private static float Ripple (Vector3 p, float t){
		p.x -= 0.5f;
		p.z -= 0.5f;
		float squareRadius = p.x * p.x + p.z * p.z;
		return 0.5f + Mathf.Sin(15f * Mathf.PI * squareRadius - 2f * t) / (2f + 100f * squareRadius);
	}
}