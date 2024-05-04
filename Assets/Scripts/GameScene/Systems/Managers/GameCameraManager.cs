﻿// File create date:2024/4/27
using System;
using System.Collections.Generic;
using Cinemachine;
using RoachFramework;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

 // Created By Yu.Liu
public class GameCameraManager : MonoSingleton<GameCameraManager> {
	
	private static readonly float ScreenRatio169 = 1920f / 1080f;
	
	public Camera MainCamera { get; private set; }
	private PixelPerfectCamera _pixelPerfectCamera;
	
	public CinemachineVirtualCamera VirtualCamera { get; private set; }

	private Square _sightLeftBot;
	private Square _sightRightTop;

	private Vector2 _camAreaLb;
	private Vector2 _camAreaRt;

	private int _zoomLevel;

	protected override void OnAwake() {
		base.OnAwake();
		MainCamera = GameObject.FindWithTag(GlobalConfigs.ObjectTagMainCamera).GetComponent<Camera>();
		_pixelPerfectCamera = MainCamera.GetComponent<PixelPerfectCamera>();
		VirtualCamera = FindComponent<CinemachineVirtualCamera>("VCam_Confine");
		_zoomLevel = 0;
	}

	public void SetupSightLimit(Square pivot, Square size) {
		var xOddOffset = 1 - size.x % 2;
		var yOddOffset = 1 - size.y % 2;
		var xHalf = size.x / 2;
		var yHalf = size.y / 2;
		_sightLeftBot = new Square(pivot.x - xHalf, pivot.y - yHalf);
		_sightRightTop = new Square(pivot.x + xHalf + xOddOffset, pivot.y + yHalf + yOddOffset);
		_camAreaLb = MapUtils.SquareToWorld(_sightLeftBot);
		_camAreaLb -= Vector2.one * 0.5f;
		_camAreaRt = MapUtils.SquareToWorld(_sightRightTop);
		_camAreaRt += Vector2.one * 0.5f;
	}

	public Vector3 ClampCameraPointPosition(Vector3 pos) {
		var xOffset = _pixelPerfectCamera.refResolutionX / 16f;
		var yOffset = xOffset / ScreenRatio169;
		var cpos = pos;
		cpos.x = Mathf.Clamp(pos.x, _camAreaLb.x + xOffset, _camAreaRt.x - xOffset);
		cpos.y = Mathf.Clamp(pos.y, _camAreaLb.y + yOffset, _camAreaRt.y - yOffset);
		return cpos;
	}

	public void SetupZoomLevel(int l) {
		if (l < 0 || l > 8) return;
		_zoomLevel = l;
		_pixelPerfectCamera.refResolutionX = 80 + _zoomLevel * 16;
		_pixelPerfectCamera.refResolutionY = 45 + _zoomLevel * 9;
	}

	public void ChangeZoomLevel(int delta) {
		SetupZoomLevel(_zoomLevel + delta);
	}
}