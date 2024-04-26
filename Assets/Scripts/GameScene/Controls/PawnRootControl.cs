// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class PawnRootControl : MonoSingleton<PawnRootControl> {

	private Dictionary<int, BasePawnController> _pawnControls;
	private int _pawnIndex;

	private int PawnIndex {
		get {
			_pawnIndex++;
			return _pawnIndex - 1;
		}
	}

	protected override void OnAwake() {
		base.OnAwake();
		_pawnControls = new Dictionary<int, BasePawnController>();
	}
}