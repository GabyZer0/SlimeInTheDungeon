﻿// This code contains NVIDIA Confidential Information and is disclosed to you
// under a form of NVIDIA software license agreement provided separately to you.
//
// Notice
// NVIDIA Corporation and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from NVIDIA Corporation is strictly prohibited.
//
// ALL NVIDIA DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". NVIDIA MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, NVIDIA Corporation assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of NVIDIA Corporation. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// NVIDIA Corporation products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// NVIDIA Corporation.
//
// Copyright (c) 2018 NVIDIA Corporation. All rights reserved.

using UnityEditor;
using UnityEngine;

namespace NVIDIA.Flex
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FlexSourceActor))]
    public class FlexSourceActorEditor : FlexActorEditor
    {
        SerializedProperty m_asset;
        //SerializedProperty m_spawnRate;
        SerializedProperty m_lifeTime;
        SerializedProperty m_startSpeed;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_asset = serializedObject.FindProperty("m_asset");
            //m_spawnRate = serializedObject.FindProperty("m_spawnRate");
            m_lifeTime = serializedObject.FindProperty("m_lifeTime");
            m_startSpeed = serializedObject.FindProperty("m_startSpeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.ContainerUI();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_asset, new GUIContent("Source Asset"));
            if (EditorGUI.EndChangeCheck()) m_recreateActor.intValue = 2;

            EditorGUILayout.Separator();

            base.ParticlesUI();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_startSpeed);
            EditorGUILayout.PropertyField(m_lifeTime);

            EditorGUILayout.Separator();

            base.DebugUI();

            if (GUI.changed) serializedObject.ApplyModifiedProperties();
        }
    }
}
