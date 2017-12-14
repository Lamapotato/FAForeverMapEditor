﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OzoneDecals;
using Ozone.UI;

namespace EditMap
{
	public class DecalSettings : MonoBehaviour
	{
		public RawImage Texture1;
		public InputField Texture1Path;
		public RawImage Texture2;
		public InputField Texture2Path;
		public UiTextField CutOff;
		public UiTextField NearCutOff;
		public Dropdown DecalType;
		public Button CreateBtn;
		public GameObject CreateSelected;

		static Decal.DecalSharedSettings Loaded;

		public static Decal.DecalSharedSettings GetLoaded
		{
			get
			{
				return Loaded;
			}
		}

		bool Loading = false;
		public void Load(Decal.DecalSharedSettings DecalSettings)
		{
			if (Creating)
				return;

			UpdateSelection();

			Loading = true;
			Loaded = DecalSettings;

			if (DecalSettings == null)
			{
				Texture1.texture = null;
				Texture1Path.text = "";

				Texture2.texture = null;
				Texture2Path.text = "";

				//CutOff.text = "500";
				//NearCutOff.text = "0";

				CreateBtn.interactable = false;
				CreateSelected.SetActive(false);
			}
			else
			{
				Texture1.texture = DecalSettings.Texture1;
				Texture1Path.text = DecalSettings.Tex1Path;

				Texture2.texture = DecalSettings.Texture2;
				Texture2Path.text = DecalSettings.Tex2Path;

				//CutOff.text = DecalSettings.CutOffLOD.ToString();
				//NearCutOff.text = DecalSettings.NearCutOffLOD.ToString();

				CreateBtn.interactable = true;

				switch (DecalSettings.Type)
				{
					case TerrainDecalType.TYPE_ALBEDO:
						DecalType.value = 0;
						break;
					case TerrainDecalType.TYPE_NORMALS:
						DecalType.value = 1;
						break;
					case TerrainDecalType.TYPE_NORMALS_ALPHA:
						DecalType.value = 2;
						break;
					case TerrainDecalType.TYPE_GLOW:
						DecalType.value = 3;
						break;
					case TerrainDecalType.TYPE_GLOW_MASK:
						DecalType.value = 4;
						break;
					case TerrainDecalType.TYPE_WATER_MASK:
						DecalType.value = 5;
						break;
					case TerrainDecalType.TYPE_WATER_ALBEDO:
						DecalType.value = 6;
						break;
					case TerrainDecalType.TYPE_WATER_NORMALS:
						DecalType.value = 7;
						break;
					case TerrainDecalType.TYPE_FORCE_DWORD:
						DecalType.value = 8;
						break;
					case TerrainDecalType.TYPE_UNDEFINED:
						DecalType.value = 9;
						break;
				}
			}
			Loading = false;
		}

		void UpdateSelection()
		{
			float NearCutoffValue = -1000;
			float CutoffValue = -100;
			HashSet<OzoneDecal>.Enumerator ListEnum = DecalsInfo.Current.SelectedDecals.GetEnumerator();
			while (ListEnum.MoveNext())
			{
				if (NearCutoffValue < 0)
					NearCutoffValue = ListEnum.Current.NearCutOffLOD;
				else
				{
					if (ListEnum.Current.NearCutOffLOD != NearCutoffValue)
					{
						// TODO Different value
					}

				}

				if (CutoffValue < 0)
					CutoffValue = ListEnum.Current.CutOffLOD;
				else
				{
					if (ListEnum.Current.CutOffLOD != CutoffValue)
					{
						// TODO Different value
					}

				}
			}

			if (NearCutoffValue < 0)
				NearCutoffValue = NearCutOff.value;

			if (CutoffValue < 0)
				CutoffValue = CutOff.value;

			NearCutOff.SetValue(NearCutoffValue);
			CutOff.SetValue(CutoffValue);
		}

		TerrainDecalType TypeByDropdown()
		{
			
			switch (DecalType.value)
			{
				case 0:
					return TerrainDecalType.TYPE_ALBEDO;
				case 1:
					return TerrainDecalType.TYPE_NORMALS;
				case 2:
					return TerrainDecalType.TYPE_NORMALS_ALPHA;
				case 3:
					return TerrainDecalType.TYPE_GLOW;
				case 4:
					return TerrainDecalType.TYPE_GLOW_MASK;
				case 5:
					return TerrainDecalType.TYPE_WATER_MASK;
				case 6:
					return TerrainDecalType.TYPE_WATER_ALBEDO;
				case 7:
					return TerrainDecalType.TYPE_WATER_NORMALS;
				case 8:
					return TerrainDecalType.TYPE_FORCE_DWORD;
				case 9:
					return TerrainDecalType.TYPE_UNDEFINED;
			}

			return TerrainDecalType.TYPE_UNDEFINED;

		}

		public void OnSelectionChanged()
		{
			CutOff.SetValue(50);
			NearCutOff.SetValue(0);
		}

		public void OnTypeChanged()
		{
			if (Loaded == null || Loading)
				return;


			if (Loaded.Type == TypeByDropdown())
				return;

			//TODO Register Undo

			Loaded.Type = TypeByDropdown();


			Loaded.UpdateMaterial();
		}


		public void ClickTex1()
		{
			if (!string.IsNullOrEmpty(Texture1Path.text))
				ResourceBrowser.Current.LoadStratumTexture(Texture1Path.text.Remove(0, 1));
			else
				ResourceBrowser.Current.gameObject.SetActive(true);
		}

		public void ClickTex2()
		{
			if (!string.IsNullOrEmpty(Texture2Path.text))
				ResourceBrowser.Current.LoadStratumTexture(Texture2Path.text.Remove(0, 1));
			else
				ResourceBrowser.Current.gameObject.SetActive(true);
		}

		public void DropTex1()
		{

			if (Loaded == null || !ResourceBrowser.Current.gameObject.activeSelf && ResourceBrowser.DragedObject)
				return;
			if (ResourceBrowser.SelectedCategory == 2)
			{
				//TODO Undo.RegisterStratumChange(Selected);
				Debug.Log(ResourceBrowser.Current.LoadedPaths[ResourceBrowser.DragedObject.InstanceId]);

				Loaded.Tex1Path = ResourceBrowser.Current.LoadedPaths[ResourceBrowser.DragedObject.InstanceId];
				Loaded.UpdateMaterial();
				Load(Loaded);
				DecalsInfo.Current.DecalsList.OnTexturesChanged();
				//ScmapEditor.Current.Textures[Selected].Albedo = ResourceBrowser.Current.LoadedTextures[ResourceBrowser.DragedObject.InstanceId];
				//ScmapEditor.Current.Textures[Selected].AlbedoPath = ResourceBrowser.Current.LoadedPaths[ResourceBrowser.DragedObject.InstanceId];

				//Map.map.Layers [Selected].PathTexture = Map.Textures [Selected].AlbedoPath;
			}
		}
		
		public void DropTex2()
		{
			if (Loaded == null || !ResourceBrowser.Current.gameObject.activeSelf && ResourceBrowser.DragedObject)
				return;
			if (ResourceBrowser.SelectedCategory == 2)
			{
				//TODO Undo.RegisterStratumChange(Selected);
				Debug.Log(ResourceBrowser.Current.LoadedPaths[ResourceBrowser.DragedObject.InstanceId]);

				Loaded.Tex2Path = ResourceBrowser.Current.LoadedPaths[ResourceBrowser.DragedObject.InstanceId];
				Loaded.UpdateMaterial();
				Load(Loaded);
				DecalsInfo.Current.DecalsList.OnTexturesChanged();
				//ScmapEditor.Current.Textures[Selected].Albedo = ResourceBrowser.Current.LoadedTextures[ResourceBrowser.DragedObject.InstanceId];
				//ScmapEditor.Current.Textures[Selected].AlbedoPath = ResourceBrowser.Current.LoadedPaths[ResourceBrowser.DragedObject.InstanceId];

				//Map.map.Layers [Selected].PathTexture = Map.Textures [Selected].AlbedoPath;
			}
		}

		public void SetTex1Path()
		{
			if (Loaded == null)
				return;

			Texture1Path.text = Loaded.Tex1Path;
		}

		public void SetTex2Path()
		{
			if (Loaded == null)
				return;

			Texture2Path.text = Loaded.Tex2Path;
		}

		public void OnCutoffLodChanged()
		{
			if (Loaded == null || Loading)
				return;

			//TODO Register Undo

			HashSet<OzoneDecal>.Enumerator ListEnum = DecalsInfo.Current.SelectedDecals.GetEnumerator();
			while (ListEnum.MoveNext())
			{
				ListEnum.Current.CutOffLOD = CutOff.value;
			}
			ListEnum.Dispose();

			UpdateSelection();
		}

		public void OnNearCutoffLodChanged()
		{
			if (Loaded == null || Loading)
				return;

			//TODO Register Undo

			HashSet<OzoneDecal>.Enumerator ListEnum = DecalsInfo.Current.SelectedDecals.GetEnumerator();
			while (ListEnum.MoveNext())
			{
				ListEnum.Current.NearCutOffLOD = NearCutOff.value;
			}
			ListEnum.Dispose();

			UpdateSelection();
		}

		public void SampleCutoffFromCamera()
		{
			float Dist = (int)CameraControler.GetCurrentZoom();
			HashSet<OzoneDecal>.Enumerator ListEnum = DecalsInfo.Current.SelectedDecals.GetEnumerator();
			while (ListEnum.MoveNext())
			{
				ListEnum.Current.CutOffLOD = Dist;
			}
			ListEnum.Dispose();
			UpdateSelection();
		}

		public void SampleNearCutoffFromCamera()
		{
			float Dist = (int)CameraControler.GetCurrentZoom();
			HashSet<OzoneDecal>.Enumerator ListEnum = DecalsInfo.Current.SelectedDecals.GetEnumerator();
			while (ListEnum.MoveNext())
			{
				ListEnum.Current.NearCutOffLOD = Dist;
			}
			ListEnum.Dispose();
			UpdateSelection();
		}


		bool Creating = false;
		public void OnClickCreate()
		{
			Creating = !Creating;
			CreateSelected.SetActive(Creating);
			if (Creating)
			{
				//TODO Enter Creating mode
				Selection.SelectionManager.Current.ClearAffectedGameObjects(false);
				PlacementManager.InstantiateAction = CreatePrefabAction;
				PlacementManager.MinRotAngle = 0;
				PlacementManager.BeginPlacement(GetCreationObject(), DecalsInfo.Current.Place);
			}
			else
			{
				//TODO Exit Creating mode
				if (CreationGameObject)
				{
					DestroyImmediate(CreationGameObject);
				}

				DecalsInfo.Current.GoToSelection();
			}
		}



		public GameObject CreationPrefab;
		GameObject CreationGameObject;
		GameObject GetCreationObject()
		{
			if (!CreationGameObject)
			{
				CreationGameObject = Instantiate(CreationPrefab);
				CreationGameObject.SetActive(false);
				CreatePrefabAction(CreationGameObject);
			}
			return CreationGameObject;
		}

		void CreatePrefabAction(GameObject InstancedPrefab)
		{
			OzoneDecal Obj = InstancedPrefab.GetComponent<OzoneDecal>();
			Decal component = new Decal();
			component.Obj = Obj;
			Obj.Dec = component;
			Obj.Dec.Shared = Loaded;
			//Dec.Shared = Loaded;
			Obj.Material = Loaded.SharedMaterial;

			Obj.CutOffLOD = CutOff.value;
			Obj.NearCutOffLOD = NearCutOff.value;
			Obj.Bake();

		}
	}

}