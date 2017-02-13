//C# Example

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MassMaterialUpdater : EditorWindow
{
    [MenuItem ("Window/Material Upater")]
    static void Init() {
      MassMaterialUpdater window = (MassMaterialUpdater)EditorWindow.GetWindow (typeof (MassMaterialUpdater));
  		window.Show();
      //set all to opaque
      /*Renderer[] rends = Resources.FindObjectsOfTypeAll(typeof(Renderer)) as Renderer[];
      foreach(Renderer rend in rends){
  			rend.material.SetFloat("_Mode", 0);
      }*/
    }
    public void OnGUI()
   {
       GUILayout.Label("Click the button to do the thing :D");


       if (GUILayout.Button("Update Materials"))
       {
           updateMaterials();
       }

   }

  void updateMaterials(){
      List<Material> armat = new List<Material>();

      Renderer[] arrend = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));
       foreach (Renderer rend in arrend)
       {
           foreach (Material mat in rend.sharedMaterials)
           {
               if (!armat.Contains(mat))
               {
                   armat.Add(mat);
               }
           }
       }

       foreach (Material mat in armat)
       {
          if(mat != null)
    			mat.SetFloat("_Mode", 0);
       }

       armat.Clear();
      //set those with object fade to cutout
      ObjectFade[] objs = Resources.FindObjectsOfTypeAll(typeof(ObjectFade)) as ObjectFade[];
      foreach(ObjectFade obj in objs){
          Renderer[] rends = obj.gameObject.GetComponentsInChildren<Renderer>();
          foreach(Renderer rend in rends){
           foreach (Material mat in rend.sharedMaterials)
           {
               if (!armat.Contains(mat))
               {
                   armat.Add(mat);
               }
           }
         }
       }

       foreach (Material mat in armat)
       {
          if(mat != null)
    			mat.SetFloat("_Mode", 1);
       }
    }
}
