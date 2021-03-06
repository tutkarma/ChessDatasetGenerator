﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Services;
using UnityEngine;
using static Assets.Scripts.Core.Helpers;

namespace Assets.Scripts.Core.Controllers
{
    public class LightController : MonoBehaviour
    {
        [SerializeField]
        private GameObject chessBoard = default;
        private readonly List<GameObject> spotLights = new List<GameObject>();

        private SettingsService settingsService;

        public void Init(SettingsService service)
        {
            settingsService = service;
        }

        public void RenderLights()
        {
            var settings = settingsService.GetRandomizedValues();
            InitLights(settings.SpotLightPositionZ,
            settings.SpotLightPositions,
            settings.SpotLightBrightness,
            settings.AmbientLightBrightness,
            settings.AmbientLightPhi);
        }

        private void InitLights(float height, IEnumerable<(float x, float y)> coordinates, float intensitySpotLight,
                                float intensityAmbientLight, float ambientLightPhi)
        {
            var (bounds, _) = GetObjectRendererParams(chessBoard);
            chessBoard.transform.position += new Vector3(0.0f, bounds.y / 2.0f, 0.0f);
            var boardWidth = bounds.x * 2;
            var rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            if (spotLights.Count != 0)
            {
                foreach (var lightGameObject in spotLights)
                {
                    Destroy(lightGameObject);
                }
            }

            // set spot light
            foreach (var (x, y) in coordinates)
            {
                var position = chessBoard.transform.position + new Vector3(GetDstFromCm(x, boardWidth), GetDstFromCm(height, boardWidth), GetDstFromCm(y,boardWidth));
                var lightGameObject = new GameObject($"SpotLight{x}:{y}");
                var lightComp = lightGameObject.AddComponent<Light>();
                lightComp.type = LightType.Spot;
                lightComp.color = Color.white;
                lightComp.intensity = intensitySpotLight;
                lightComp.shadows = LightShadows.Soft;
                lightGameObject.transform.position = position;
                lightGameObject.transform.rotation = rotation;
                spotLights.Add(lightGameObject);
            }

            // set directional light
            var radiusCoefficient = 100f;
            var lightPosition = new Vector3(radiusCoefficient * Convert.ToSingle(Math.Cos(ambientLightPhi * Math.PI / 180.0)),
                300,
                -radiusCoefficient * Convert.ToSingle(Math.Sin(ambientLightPhi * Math.PI / 180.0)));
            rotation = Quaternion.Euler(60.0f, ambientLightPhi, 0.0f);
            var lightComponent = gameObject.GetComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.color = Color.white;
            lightComponent.shadows = LightShadows.Soft;
            lightComponent.intensity = intensityAmbientLight;
            gameObject.transform.position = lightPosition;
            gameObject.transform.rotation = rotation;
        }
    }
}
