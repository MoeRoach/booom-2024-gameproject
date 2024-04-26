using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 通用UI动画控制器的编辑器拓展
    /// </summary>
    [CustomEditor(typeof(UiAnimator), true)]
    public class UiAnimatorEditor : CustomScriptEditor {
        
        private SerializedProperty _hasShowAnimationProperty;
		private UiAnimationDataProperty _showAnimationProperty;
		private SerializedProperty _hasHideAnimationProperty;
		private UiAnimationDataProperty _hideAnimationProperty;
		
		private SerializedProperty _independentDeltaTimeProperty;
		
		private Color _moveAnimColor;
		private Color _rotateAnimColor;
		private Color _scaleAnimColor;
		private Color _fadeAnimColor;
		
		protected override void OnInit() {
			_moveAnimColor = Color.green;
			_moveAnimColor.a = 0.6f;
			_rotateAnimColor = Color.magenta;
			_rotateAnimColor.a = 0.6f;
			_scaleAnimColor = Color.red;
			_scaleAnimColor.a = 0.6f;
			_fadeAnimColor = Color.yellow;
			_fadeAnimColor.a = 0.6f;
			_hasShowAnimationProperty = GetPropertyFromTarget("hasShowAnimation");
			_showAnimationProperty =
				new UiAnimationDataProperty(GetPropertyFromTarget("showAnimation"));
			_hasHideAnimationProperty = GetPropertyFromTarget("hasHideAnimation");
			_hideAnimationProperty =
				new UiAnimationDataProperty(GetPropertyFromTarget("hideAnimation"));
			_independentDeltaTimeProperty = GetPropertyFromTarget("independentDeltaTime");
		}

		protected override void OnCustomInspectorGUI() {
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("=== UI动画配置 ===");
			
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.BeginHorizontal();
			_hasShowAnimationProperty.boolValue = EditorGUILayout.Toggle(
				_hasShowAnimationProperty.boolValue, GUILayout.Width(14f));
			EditorGUILayout.LabelField("UI展示动画");
			EditorGUILayout.EndHorizontal();
			if (_hasShowAnimationProperty.boolValue) {
				DrawAnimationDataProperty(_showAnimationProperty);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.BeginHorizontal();
			_hasHideAnimationProperty.boolValue = EditorGUILayout.Toggle(
				_hasHideAnimationProperty.boolValue, GUILayout.Width(14f));
			EditorGUILayout.LabelField("UI隐藏动画");
			EditorGUILayout.EndHorizontal();
			if (_hasHideAnimationProperty.boolValue) {
				DrawAnimationDataProperty(_hideAnimationProperty);
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginHorizontal();
			_independentDeltaTimeProperty.boolValue = EditorGUILayout.Toggle(
				_independentDeltaTimeProperty.boolValue, GUILayout.Width(14f));
			EditorGUILayout.LabelField("使用独立的DeltaTime");
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndVertical();
		}

		private void DrawAnimationDataProperty(UiAnimationDataProperty prop) {
			EditorGUILayout.BeginHorizontal();
			prop.hasMoveProperty.boolValue = EditorGUILayout.Toggle(
				prop.hasMoveProperty.boolValue, GUILayout.Width(14f));
			EditorGUILayout.LabelField("位移", GUILayout.Width(28f));
			EditorGUILayout.Space();
			prop.hasRotateProperty.boolValue = EditorGUILayout.Toggle(
				prop.hasRotateProperty.boolValue, GUILayout.Width(14f));
			EditorGUILayout.LabelField("旋转", GUILayout.Width(28f));
			EditorGUILayout.Space();
			prop.hasScaleProperty.boolValue = EditorGUILayout.Toggle(
				prop.hasScaleProperty.boolValue, GUILayout.Width(14f));
			EditorGUILayout.LabelField("缩放", GUILayout.Width(28f));
			EditorGUILayout.Space();
			prop.hasFadeProperty.boolValue = EditorGUILayout.Toggle(
				prop.hasFadeProperty.boolValue, GUILayout.Width(14f));
			EditorGUILayout.LabelField("透明", GUILayout.Width(28f));
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
			if (prop.hasMoveProperty.boolValue) {
                //EditorGUILayout.LabelField("位移动画信息");
                GUI.backgroundColor = _moveAnimColor;
                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUI.backgroundColor = Color.white;
                // delay duration relative
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("延迟", GUILayout.Width(28f));
                prop.moveAnimationProperty.startDelayProperty.floatValue = EditorGUILayout.FloatField(prop.moveAnimationProperty.startDelayProperty.floatValue, GUILayout.Width(56f));
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("时长", GUILayout.Width(28f));
                prop.moveAnimationProperty.animDurationProperty.floatValue = EditorGUILayout.FloatField(prop.moveAnimationProperty.animDurationProperty.floatValue, GUILayout.Width(56f));
                EditorGUILayout.Space();
                prop.moveAnimationProperty.relativeAnimProperty.boolValue = EditorGUILayout.Toggle(prop.moveAnimationProperty.relativeAnimProperty.boolValue, GUILayout.Width(14f));
                EditorGUILayout.LabelField("相对变化", GUILayout.Width(56f));
                EditorGUILayout.EndHorizontal();
                // from
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("起始坐标：", GUILayout.Width(70f));
                EditorGUILayout.LabelField("X", GUILayout.Width(14f));
                var xFrom = EditorGUILayout.FloatField(prop.moveAnimationProperty.moveFromProperty.vector2Value.x);
                EditorGUILayout.LabelField("Y", GUILayout.Width(14f));
                var yFrom = EditorGUILayout.FloatField(prop.moveAnimationProperty.moveFromProperty.vector2Value.y);
                prop.moveAnimationProperty.moveFromProperty.vector2Value = new Vector2(xFrom, yFrom);
                EditorGUILayout.EndHorizontal();
                // to
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目的坐标：", GUILayout.Width(70f));
                EditorGUILayout.LabelField("X", GUILayout.Width(14f));
                var xTo = EditorGUILayout.FloatField(prop.moveAnimationProperty.moveToProperty.vector2Value.x);
                EditorGUILayout.LabelField("Y", GUILayout.Width(14f));
                var yTo = EditorGUILayout.FloatField(prop.moveAnimationProperty.moveToProperty.vector2Value.y);
                prop.moveAnimationProperty.moveToProperty.vector2Value = new Vector2(xTo, yTo);
                EditorGUILayout.EndHorizontal();
                // ease
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("变化方式：", GUILayout.Width(70f));
                Enum ease = (AnimationEase)prop.moveAnimationProperty.animEaseProperty.enumValueIndex;
                ease = EditorGUILayout.EnumPopup(ease);
                prop.moveAnimationProperty.animEaseProperty.enumValueIndex = Convert.ToInt32(ease);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            if (prop.hasRotateProperty.boolValue) {
                //EditorGUILayout.LabelField("旋转动画信息");
                GUI.backgroundColor = _rotateAnimColor;
                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUI.backgroundColor = Color.white;
                // delay duration relative
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("延迟", GUILayout.Width(28f));
                prop.rotateAnimationProperty.startDelayProperty.floatValue = EditorGUILayout.FloatField(prop.rotateAnimationProperty.startDelayProperty.floatValue, GUILayout.Width(56f));
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("时长", GUILayout.Width(28f));
                prop.rotateAnimationProperty.animDurationProperty.floatValue = EditorGUILayout.FloatField(prop.rotateAnimationProperty.animDurationProperty.floatValue, GUILayout.Width(56f));
                EditorGUILayout.Space();
                prop.rotateAnimationProperty.relativeAnimProperty.boolValue = EditorGUILayout.Toggle(prop.rotateAnimationProperty.relativeAnimProperty.boolValue, GUILayout.Width(14f));
                EditorGUILayout.LabelField("相对变化", GUILayout.Width(56f));
                EditorGUILayout.EndHorizontal();
                // from
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("起始角度：", GUILayout.Width(70f));
                EditorGUILayout.LabelField("X", GUILayout.Width(14f));
                var xFrom = EditorGUILayout.FloatField(prop.rotateAnimationProperty.rotateFromProperty.vector3Value.x);
                EditorGUILayout.LabelField("Y", GUILayout.Width(14f));
                var yFrom = EditorGUILayout.FloatField(prop.rotateAnimationProperty.rotateFromProperty.vector3Value.y);
                EditorGUILayout.LabelField("Z", GUILayout.Width(14f));
                var zFrom = EditorGUILayout.FloatField(prop.rotateAnimationProperty.rotateFromProperty.vector3Value.z);
                prop.rotateAnimationProperty.rotateFromProperty.vector3Value = new Vector3(xFrom, yFrom, zFrom);
                EditorGUILayout.EndHorizontal();
                // to
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目的角度：", GUILayout.Width(70f));
                EditorGUILayout.LabelField("X", GUILayout.Width(14f));
                var xTo = EditorGUILayout.FloatField(prop.rotateAnimationProperty.rotateToProperty.vector3Value.x);
                EditorGUILayout.LabelField("Y", GUILayout.Width(14f));
                var yTo = EditorGUILayout.FloatField(prop.rotateAnimationProperty.rotateToProperty.vector3Value.y);
                EditorGUILayout.LabelField("Z", GUILayout.Width(14f));
                var zTo = EditorGUILayout.FloatField(prop.rotateAnimationProperty.rotateToProperty.vector3Value.z);
                prop.rotateAnimationProperty.rotateToProperty.vector3Value = new Vector3(xTo, yTo, zTo);
                EditorGUILayout.EndHorizontal();
                // ease
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("变化方式：", GUILayout.Width(70f));
                Enum ease = (AnimationEase)prop.rotateAnimationProperty.animEaseProperty.enumValueIndex;
                ease = EditorGUILayout.EnumPopup(ease);
                prop.rotateAnimationProperty.animEaseProperty.enumValueIndex = Convert.ToInt32(ease);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            if (prop.hasScaleProperty.boolValue) {
                //EditorGUILayout.LabelField("缩放动画信息");
                GUI.backgroundColor = _scaleAnimColor;
                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUI.backgroundColor = Color.white;
                // delay duration relative
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("延迟", GUILayout.Width(28f));
                prop.scaleAnimationProperty.startDelayProperty.floatValue = EditorGUILayout.FloatField(prop.scaleAnimationProperty.startDelayProperty.floatValue, GUILayout.Width(56f));
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("时长", GUILayout.Width(28f));
                prop.scaleAnimationProperty.animDurationProperty.floatValue = EditorGUILayout.FloatField(prop.scaleAnimationProperty.animDurationProperty.floatValue, GUILayout.Width(56f));
                EditorGUILayout.Space();
                prop.scaleAnimationProperty.relativeAnimProperty.boolValue = EditorGUILayout.Toggle(prop.scaleAnimationProperty.relativeAnimProperty.boolValue, GUILayout.Width(14f));
                EditorGUILayout.LabelField("相对变化", GUILayout.Width(56f));
                EditorGUILayout.EndHorizontal();
                // from
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("起始缩放：", GUILayout.Width(70f));
                EditorGUILayout.LabelField("X", GUILayout.Width(14f));
                var xFrom = EditorGUILayout.FloatField(prop.scaleAnimationProperty.scaleFromProperty.vector3Value.x);
                EditorGUILayout.LabelField("Y", GUILayout.Width(14f));
                var yFrom = EditorGUILayout.FloatField(prop.scaleAnimationProperty.scaleFromProperty.vector3Value.y);
                EditorGUILayout.LabelField("Z", GUILayout.Width(14f));
                var zFrom = EditorGUILayout.FloatField(prop.scaleAnimationProperty.scaleFromProperty.vector3Value.z);
                prop.scaleAnimationProperty.scaleFromProperty.vector3Value = new Vector3(xFrom, yFrom, zFrom);
                EditorGUILayout.EndHorizontal();
                // to
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目的缩放：", GUILayout.Width(70f));
                EditorGUILayout.LabelField("X", GUILayout.Width(14f));
                var xTo = EditorGUILayout.FloatField(prop.scaleAnimationProperty.scaleToProperty.vector3Value.x);
                EditorGUILayout.LabelField("Y", GUILayout.Width(14f));
                var yTo = EditorGUILayout.FloatField(prop.scaleAnimationProperty.scaleToProperty.vector3Value.y);
                EditorGUILayout.LabelField("Z", GUILayout.Width(14f));
                var zTo = EditorGUILayout.FloatField(prop.scaleAnimationProperty.scaleToProperty.vector3Value.z);
                prop.scaleAnimationProperty.scaleToProperty.vector3Value = new Vector3(xTo, yTo, zTo);
                EditorGUILayout.EndHorizontal();
                // ease
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("变化方式：", GUILayout.Width(70f));
                Enum ease = (AnimationEase)prop.scaleAnimationProperty.animEaseProperty.enumValueIndex;
                ease = EditorGUILayout.EnumPopup(ease);
                prop.scaleAnimationProperty.animEaseProperty.enumValueIndex = Convert.ToInt32(ease);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            if (prop.hasFadeProperty.boolValue) {
                //EditorGUILayout.LabelField("透明动画信息");
                GUI.backgroundColor = _fadeAnimColor;
                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUI.backgroundColor = Color.white;
                // delay duration relative
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("延迟", GUILayout.Width(28f));
                prop.fadeAnimationProperty.startDelayProperty.floatValue = EditorGUILayout.FloatField(prop.fadeAnimationProperty.startDelayProperty.floatValue, GUILayout.Width(56f));
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("时长", GUILayout.Width(28f));
                prop.fadeAnimationProperty.animDurationProperty.floatValue = EditorGUILayout.FloatField(prop.fadeAnimationProperty.animDurationProperty.floatValue, GUILayout.Width(56f));
                EditorGUILayout.Space();
                prop.fadeAnimationProperty.relativeAnimProperty.boolValue = EditorGUILayout.Toggle(prop.fadeAnimationProperty.relativeAnimProperty.boolValue, GUILayout.Width(14f));
                EditorGUILayout.LabelField("相对变化", GUILayout.Width(56f));
                EditorGUILayout.EndHorizontal();
                // from
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("起始透明：", GUILayout.Width(70f));
                prop.fadeAnimationProperty.fadeFromProperty.floatValue = EditorGUILayout.FloatField(prop.fadeAnimationProperty.fadeFromProperty.floatValue);
                EditorGUILayout.EndHorizontal();
                // to
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目的透明：", GUILayout.Width(70f));
                prop.fadeAnimationProperty.fadeToProperty.floatValue = EditorGUILayout.FloatField(prop.fadeAnimationProperty.fadeToProperty.floatValue);
                EditorGUILayout.EndHorizontal();
                // ease
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("变化方式：", GUILayout.Width(70f));
                Enum ease = (AnimationEase)prop.fadeAnimationProperty.animEaseProperty.enumValueIndex;
                ease = EditorGUILayout.EnumPopup(ease);
                prop.fadeAnimationProperty.animEaseProperty.enumValueIndex = Convert.ToInt32(ease);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
		}
    }
    
    public class UiAnimationDataProperty {
        public SerializedProperty hasMoveProperty;
        public MoveAnimationProperty moveAnimationProperty;
        public SerializedProperty hasRotateProperty;
        public RotateAnimationProperty rotateAnimationProperty;
        public SerializedProperty hasScaleProperty;
        public ScaleAnimationProperty scaleAnimationProperty;
        public SerializedProperty hasFadeProperty;
        public FadeAnimationProperty fadeAnimationProperty;

        public UiAnimationDataProperty(SerializedProperty prop) {
            hasMoveProperty = prop.FindPropertyRelative("hasMove");
            moveAnimationProperty =
	            new MoveAnimationProperty(prop.FindPropertyRelative("moveAnimation"));
            hasRotateProperty = prop.FindPropertyRelative("hasRotate");
            rotateAnimationProperty =
	            new RotateAnimationProperty(prop.FindPropertyRelative("rotateAnimation"));
            hasScaleProperty = prop.FindPropertyRelative("hasScale");
            scaleAnimationProperty =
	            new ScaleAnimationProperty(prop.FindPropertyRelative("scaleAnimation"));
            hasFadeProperty = prop.FindPropertyRelative("hasFade");
            fadeAnimationProperty =
	            new FadeAnimationProperty(prop.FindPropertyRelative("fadeAnimation"));
        }
    }

    public class MoveAnimationProperty : BaseAnimationProperty {
        public SerializedProperty moveFromProperty;
        public SerializedProperty moveToProperty;

        public MoveAnimationProperty(SerializedProperty prop) : base(prop) {
            moveFromProperty = prop.FindPropertyRelative("moveFrom");
            moveToProperty = prop.FindPropertyRelative("moveTo");
        }
    }

    public class RotateAnimationProperty : BaseAnimationProperty {
        public SerializedProperty rotateFromProperty;
        public SerializedProperty rotateToProperty;

        public RotateAnimationProperty(SerializedProperty prop) : base(prop) {
            rotateFromProperty = prop.FindPropertyRelative("rotateFrom");
            rotateToProperty = prop.FindPropertyRelative("rotateTo");
        }
    }

    public class ScaleAnimationProperty : BaseAnimationProperty {
        public SerializedProperty scaleFromProperty;
        public SerializedProperty scaleToProperty;

        public ScaleAnimationProperty(SerializedProperty prop) : base(prop) {
            scaleFromProperty = prop.FindPropertyRelative("scaleFrom");
            scaleToProperty = prop.FindPropertyRelative("scaleTo");
        }
    }

    public class FadeAnimationProperty : BaseAnimationProperty {
        public SerializedProperty fadeFromProperty;
        public SerializedProperty fadeToProperty;

        public FadeAnimationProperty(SerializedProperty prop) : base(prop) {
            fadeFromProperty = prop.FindPropertyRelative("fadeFrom");
            fadeToProperty = prop.FindPropertyRelative("fadeTo");
        }
    }

    public class BaseAnimationProperty {
        public SerializedProperty startDelayProperty;
        public SerializedProperty animDurationProperty;
        public SerializedProperty relativeAnimProperty;
        public SerializedProperty animEaseProperty;

        public BaseAnimationProperty(SerializedProperty prop) {
            startDelayProperty = prop.FindPropertyRelative("startDelay");
            animDurationProperty = prop.FindPropertyRelative("animDuration");
            relativeAnimProperty = prop.FindPropertyRelative("relativeAnim");
            animEaseProperty = prop.FindPropertyRelative("animationEase");
        }
    }
}
