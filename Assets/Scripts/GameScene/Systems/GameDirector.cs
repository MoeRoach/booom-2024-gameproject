// File create date:2024/5/2
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class GameDirector {

	private GameRootControl _gameRoot;
	private PawnRootControl _pawnRoot;
	private ObjectRootControl _objectRoot;
	private FacilityRootControl _facilityRoot;

	private GameDataService _dataService;

	public GameDirector(GameRootControl root) {
		_gameRoot = root;
	}

	public void Init() {
		_dataService = GameSystem.Instance.GameData;
		_pawnRoot = PawnRootControl.Instance;
		_objectRoot = ObjectRootControl.Instance;
		_facilityRoot = FacilityRootControl.Instance;
	}

	public void SetupBroadcastFilter(BroadcastFilter filter) {
		
	}

	public void UpdateDirector() {
		
	}

	public void ReceiveBroadcast(BroadcastInfo info) {
		
	}
}