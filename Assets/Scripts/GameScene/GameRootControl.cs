﻿// File create date:2024/4/19
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using RoachFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// Created By Yu.Liu
public class GameRootControl : MonoSingleton<GameRootControl> {

	private Tilemap _background;
	private Tilemap _terrain;
	private Tilemap _structure;

	private Transform _cameraPoint;
	private bool isDragging;
	private Vector3 startPointerPosition;
	private Vector3 realPointerPosition;

	private GameDataService _gameData;

	protected override void OnAwake() {
		base.OnAwake();
		_background = FindComponent<Tilemap>("MapRoot.Background");
		_terrain = FindComponent<Tilemap>("MapRoot.Terrain");
		_structure = FindComponent<Tilemap>("MapRoot.Structure");
		_cameraPoint = FindComponent<Transform>("CameraPoint");
	}

	protected override void OnStart() {
		base.OnStart();
		_gameData = GameSystem.Instance.GameData;
		var sightArea = new Square(24, 16);
		GameCameraManager.Instance.SetupSightLimit(Square.Zero, sightArea);
		AsyncTask("InitStage", AsyncInitStage);
		RegisterUpdateFunction(1, UpdateCamera);
	}
	
	private void UpdateCamera() {
		if (Mouse.current.rightButton.wasPressedThisFrame) {
			isDragging = true;
			var mpos = Mouse.current.position.ReadValue();
			startPointerPosition = GameCameraManager.Instance.MainCamera.ScreenToWorldPoint(mpos);
			startPointerPosition.z = 0f;
		} else if (Mouse.current.rightButton.isPressed && isDragging) {
			var mpos = Mouse.current.position.ReadValue();
			realPointerPosition = GameCameraManager.Instance.MainCamera.ScreenToWorldPoint(mpos);
			realPointerPosition.z = 0f;
			var translateDir = realPointerPosition - startPointerPosition;
			var cameraPosition = _cameraPoint.position - translateDir;
			_cameraPoint.position = GameCameraManager.Instance.ClampCameraPointPosition(cameraPosition);
		} else if (Mouse.current.rightButton.wasReleasedThisFrame) {
			isDragging = false;
		}
		
		// var scrollDis = Mouse.current.scroll.ReadValue().y / 64f;
		// var isOverCanvas = EventSystem.current.IsPointerOverGameObject();
		// if (Mathf.Approximately(scrollDis, 0f) || isOverCanvas) return;
		// var cameraSize = mainCamera.orthographicSize;
		// scrollDis *= zoomMultiplier;
		// cameraSize = Mathf.Clamp(cameraSize - scrollDis, 2f, 16f);
		// mainCamera.orthographicSize = cameraSize;
	}

	private async UniTask AsyncInitStage(CancellationTokenSource cts) {
		await UniTask.NextFrame(cts.Token);
		// 先构造关卡数据
		GenerateStageData();
		await UniTask.Yield(cts.Token);
		// 根据数据布局整个地图，包括物体的生成
		_gameData.stageGenerated = true;
	}

	private void GenerateStageData() {
		_gameData.stageData.size = new Square(41, 31);
		for (var x = -20; x <= 20; x++) {
			for (var y = -15; y <= 15; y++) {
				var sq = new Square(x, y);
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
			}
		}
	}
}