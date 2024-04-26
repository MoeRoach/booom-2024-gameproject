using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 输入管理器
    /// </summary>
    public class InputManager : MonoSingleton<InputManager> {
        /// <summary>
		/// 按钮输入状态
		/// </summary>
		private enum InputState { None, Pressed, Held, Released }
		
		private class Axis {
			public readonly string name;
			public readonly List<AxisInput> inputs;

			public float value = 0f;
			public float valueRaw = 0f;

			public Axis(string axis) {
				name = axis;
				inputs = new List<AxisInput>();
			}
		}
		
		private class Button {
			public readonly string name;
			public readonly List<ButtonInput> inputs;

			public InputState state = InputState.None;

			public Button(string button) {
				name = button;
				inputs = new List<ButtonInput>();
			}
		}

		private class Vec2 {
			public readonly string name;
			public readonly List<Vector2Input> inputs;

			public Vector2 value = Vector2.zero;

			public Vec2(string vec) {
				name = vec;
				inputs = new List<Vector2Input>();
			}
		}

		private class Vec3 {
			public readonly string name;
			public readonly List<Vector3Input> inputs;
			
			public Vector3 value = Vector3.zero;

			public Vec3(string vec) {
				name = vec;
				inputs = new List<Vector3Input>();
			}
		}
		
		public static float GetAxisSensitivity = 20f;
		public static float GetAxisDeadZone = 0.025f;
		public static bool GetAxisSnapValue = true;
		public static bool GetAxisTimeScaleDependent = true;

		private Dictionary<string, Axis> _inputAxises = new Dictionary<string, Axis>();
		private List<Axis> _axisList = new List<Axis>();
		private Dictionary<string, Button> _inputButtons = new Dictionary<string, Button>();
		private List<Button> _buttonList = new List<Button>();
		private Dictionary<string, Vec2> _inputVec2 = new Dictionary<string, Vec2>();
		private List<Vec2> _vec2List = new List<Vec2>();
		private Dictionary<string, Vec3> _inputVec3 = new Dictionary<string, Vec3>();
		private List<Vec3> _vec3List = new List<Vec3>();

		public event Action OnInputUpdate;
		public event Action OnInputUpdateDone;

		public float GetAxis(string key) {
			var axis = _inputAxises.TryGetElement(key);
			return axis?.value ?? 0f;
		}

		public float GetAxisRaw(string key) {
			var axis = _inputAxises.TryGetElement(key);
			return axis?.valueRaw ?? 0f;
		}

		public bool GetButtonDown(string key) {
			var button = _inputButtons.TryGetElement(key);
			return button != null && button.state == InputState.Pressed;
		}

		public bool GetButton(string key) {
			var button = _inputButtons.TryGetElement(key);
			return button != null &&
			       (button.state == InputState.Pressed || button.state == InputState.Held);
		}

		public bool GetButtonUp(string key) {
			var button = _inputButtons.TryGetElement(key);
			return button != null && button.state == InputState.Released;
		}

		public void RegisterAxis(AxisInput input) {
			var axis = _inputAxises.TryGetElement(input.Key);
			if (axis == null) {
				axis = new Axis(input.Key);
				_inputAxises[axis.name] = axis;
				_axisList.Add(axis);
			}
			axis.inputs.Add(input);
		}

		public void UnregisterAxis(AxisInput input) {
			var axis = _inputAxises.TryGetElement(input.Key);
			if (axis == null) return;
			axis.inputs.Remove(input);
			if (axis.inputs.Count > 0) return;
			_inputAxises.Remove(axis.name);
			_axisList.Remove(axis);
		}

		public void RegisterButton(ButtonInput input) {
			var button = _inputButtons.TryGetElement(input.Key);
			if (button == null) {
				button = new Button(input.Key);
				_inputButtons[button.name] = button;
				_buttonList.Add(button);
			}
			button.inputs.Add(input);
		}

		public void UnregisterButton(ButtonInput input) {
			var button = _inputButtons.TryGetElement(input.Key);
			if (button == null) return;
			button.inputs.Remove(input);
			if (button.inputs.Count > 0) return;
			_inputButtons.Remove(button.name);
			_buttonList.Remove(button);
		}
		
		public void RegisterVec2(Vector2Input input) {
			var vec = _inputVec2.TryGetElement(input.Key);
			if (vec == null) {
				vec = new Vec2(input.Key);
				_inputVec2[vec.name] = vec;
				_vec2List.Add(vec);
			}
			vec.inputs.Add(input);
		}

		public void UnregisterVec2(Vector2Input input) {
			var vec = _inputVec2.TryGetElement(input.Key);
			if (vec == null) return;
			vec.inputs.Remove(input);
			if (vec.inputs.Count > 0) return;
			_inputVec2.Remove(vec.name);
			_vec2List.Remove(vec);
		}
		
		public void RegisterVec3(Vector3Input input) {
			var vec = _inputVec3.TryGetElement(input.Key);
			if (vec == null) {
				vec = new Vec3(input.Key);
				_inputVec3[vec.name] = vec;
				_vec3List.Add(vec);
			}
			vec.inputs.Add(input);
		}

		public void UnregisterVec3(Vector3Input input) {
			var vec = _inputVec3.TryGetElement(input.Key);
			if (vec == null) return;
			vec.inputs.Remove(input);
			if (vec.inputs.Count > 0) return;
			_inputVec3.Remove(vec.name);
			_vec3List.Remove(vec);
		}

		protected sealed override void OnUpdate() {
			base.OnUpdate();
			OnInputUpdate?.Invoke();
			var sensitivity = GetAxisSensitivity * (GetAxisTimeScaleDependent
				? Time.deltaTime
				: Time.unscaledDeltaTime);
			for (var i = 0; i < _axisList.Count; i++) {
				var axis = _axisList[i];
				axis.valueRaw = 0f;
				for (var ai = axis.inputs.Count - 1; ai >= 0; ai--) {
					var input = axis.inputs[ai];
					if (!input.Active || Mathf.Approximately(input.Value, 0f)) continue;
					axis.valueRaw = input.Value;
					break;
				}

				if (axis.value >= 0f) {
					axis.value = Mathf.Lerp(axis.valueRaw >= 0f || !GetAxisSnapValue
						? axis.value
						: 0, axis.valueRaw, sensitivity);
				} else {
					axis.value = Mathf.Lerp(axis.valueRaw <= 0f || !GetAxisSnapValue
						? axis.value
						: 0, axis.valueRaw, sensitivity);
				}

				if (!Mathf.Approximately(axis.valueRaw, 0f) ||
				    Mathf.Approximately(axis.value, 0f)) continue;
				if (Mathf.Abs(axis.value) < GetAxisDeadZone)
					axis.value = 0f;
			}

			for (var i = 0; i < _buttonList.Count; i++) {
				var button = _buttonList[i];
				var down = false;
				for (var bi = button.inputs.Count - 1; bi >= 0; bi--) {
					var input = button.inputs[bi];
					if(!input.Active || !input.Value) continue;
					down = true;
					break;
				}

				if (down) {
					if (button.state == InputState.None || button.state == InputState.Released)
						button.state = InputState.Pressed;
					else
						button.state = InputState.Held;
				} else {
					if (button.state == InputState.Pressed || button.state == InputState.Held)
						button.state = InputState.Released;
					else
						button.state = InputState.None;
				}
			}

			for (var i = 0; i < _vec2List.Count; i++) {
				var vec = _vec2List[i];
				var v = Vector2.zero;
				for (var vi = vec.inputs.Count - 1; vi >= 0; vi--) {
					var input = vec.inputs[vi];
					if(!input.Active || input.Value == Vector2.zero) continue;
					v = input.Value;
					break;
				}

				vec.value = v;
			}

			for (var i = 0; i < _vec3List.Count; i++) {
				var vec = _vec3List[i];
				var v = Vector3.zero;
				for (var vi = vec.inputs.Count - 1; vi >= 0; vi--) {
					var input = vec.inputs[vi];
					if(!input.Active || input.Value == Vector3.zero) continue;
					v = input.Value;
					break;
				}

				vec.value = v;
			}
			
			OnInputUpdateDone?.Invoke();
		}
    }
    
    /// <summary>
    /// 虚拟输入组件接口
    /// </summary>
    public interface IVirtualInput {
	    void SetupActive(bool active);
	    object ReadValue();
	    void Reset();
    }

    /// <summary>
    /// 基础虚拟输入组件的抽象基类
    /// </summary>
    /// <typeparam name="TKey">输入索引类型</typeparam>
    /// <typeparam name="TValue">输入值类型</typeparam>
    public abstract class BaseInput<TKey, TValue> : IVirtualInput {

	    public TKey Key { get; protected set; }
	    public TValue Value { get; set; }
	    public bool Active { get; protected set; }
		
	    public BaseInput() {
		    Active = true;
	    }

	    public BaseInput(TKey key) {
		    Key = key;
		    Active = true;
	    }

	    public void SetupActive(bool active) {
		    Active = active;
	    }

	    public object ReadValue() {
		    return Value;
	    }

	    public virtual void Reset() {
		    Value = default;
	    }
    }

    public class AxisInput : BaseInput<string, float> {
	    public AxisInput() : base() { }
	    public AxisInput(string key) : base(key) { }
    }
	
    public class ButtonInput : BaseInput<string, bool> {
	    public ButtonInput() : base() { }
	    public ButtonInput(string key) : base(key) { }
    }

    public class Vector2Input : BaseInput<string, Vector2> {
	    public Vector2Input() : base() { }
	    public Vector2Input(string key) : base(key) { }
    }

    public class Vector3Input : BaseInput<string, Vector3> {
	    public Vector3Input() : base() { }
	    public Vector3Input(string key) : base(key) { }
    }
}
