// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class PawnRootControl : MonoSingleton<PawnRootControl> {

	private Dictionary<int, BasePawnController> _pawnControls;
	private int _pawnIndex;

	public BasePawnController this[int id] => _pawnControls?.TryGetElement(id);

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

	public void SpawnPawn(string pn, Square coord, Square area, int i = 0) {
		var po = PrefabUtils.CreateStagePawn(transform, pn, i);
		var ctrl = po.GetComponent<BasePawnController>();
		ctrl.RegisterDestroyCallback(OnPawnDead);
		ctrl.SetupIdentifier(PawnIndex);
		ctrl.SetupArea(area);
		ctrl.SetupCoord(coord);
		ctrl.NotifyGenerate();
		_pawnControls[ctrl.Id] = ctrl;
	}

	private void OnPawnDead(GameObject po) {
		var ctrl = po.GetComponent<BasePawnController>();
		if (ctrl == null) return;
		_pawnControls.Remove(ctrl.Id);
	}
}