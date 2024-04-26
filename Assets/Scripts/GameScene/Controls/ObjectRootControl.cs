// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class ObjectRootControl : MonoSingleton<ObjectRootControl> {

	private Dictionary<int, BaseObjectControl> _objectControls;
	private int _objectIndex;

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
}