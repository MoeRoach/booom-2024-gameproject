﻿// File create date:2024/4/27
using System;
using System.Collections.Generic;
using Cinemachine;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class GameCameraManager : MonoSingleton<GameCameraManager> {
	
	public Camera MainCamera { get; private set; }
	
	public CinemachineVirtualCamera VirtualCamera { get; private set; }

	private Square _sightLeftBot;
	private Square _sightRightTop;

	private Vector2 _camAreaLb;
	private Vector2 _camAreaRt;

	protected override void OnAwake() {
		base.OnAwake();
		MainCamera = GameObject.FindWithTag(GlobalConfigs.ObjectTagMainCamera).GetComponent<Camera>();
		VirtualCamera = FindComponent<CinemachineVirtualCamera>("VCam_Confine");
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
		var yOffset = MainCamera.orthographicSize;
		var xOffset = Screen.width * 1f / Screen.height * yOffset;
		var cpos = pos;
		cpos.x = Mathf.Clamp(pos.x, _camAreaLb.x + xOffset, _camAreaRt.x - xOffset);
		cpos.y = Mathf.Clamp(pos.y, _camAreaLb.y + yOffset, _camAreaRt.y - yOffset);
		return cpos;
	}
}