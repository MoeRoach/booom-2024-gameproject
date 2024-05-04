// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class ObjectRootControl : MonoSingleton<ObjectRootControl> {

	private Dictionary<int, BaseObjectControl> _objectControls;
	private int _objectIndex;

	public BaseObjectControl this[int id] => _objectControls?.TryGetElement(id);

	private int ObjectIndex {
		get {
			_objectIndex++;
			return _objectIndex - 1;
		}
	}

	protected override void OnAwake() {
		base.OnAwake();
		_objectControls = new Dictionary<int, BaseObjectControl>();
	}
	
	public void SpawnObject(string on, Square coord, int i = 0) {
		var oo = PrefabUtils.CreateStageObject(transform, on, i);
		var ctrl = oo.GetComponent<BaseObjectControl>();
		ctrl.RegisterDestroyCallback(OnObjectDead);
		ctrl.SetupIdentifier(ObjectIndex);
		ctrl.SetupCoord(coord);
		ctrl.NotifyGenerate();
		_objectControls[ctrl.Id] = ctrl;
	}

	private void OnObjectDead(GameObject oo) {
		var ctrl = oo.GetComponent<BaseObjectControl>();
		if (ctrl == null) return;
		_objectControls.Remove(ctrl.Id);
	}
}