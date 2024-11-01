using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefabModifier : MonoBehaviour
{
    [MenuItem("Tools/Modificar Prefab")]
    public static void Run()
    {
        // Ruta de la carpeta que deseas recorrer
        string folderPath = "Assets/Project/Resources/Rooms/AllRooms";

        // Obtener todos los archivos .prefab en la carpeta especificada
        string[] prefabFiles = Directory.GetFiles(folderPath, "*.prefab", SearchOption.TopDirectoryOnly);

        // Recorrer cada archivo prefab y aplicar la modificación
        foreach (string prefabFile in prefabFiles)
        {
            //ModifyEnemy(prefabFile);
            ChangeNamePrefab(prefabFile);
        }
    }

    private static void ChangeNamePrefab(string prefabPath)
    {
        // Cargar el prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab != null)
        {
            // Instanciar el prefab para modificarlo
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            // Buscar el GameObject hijo llamado "Enemies"
            Transform enemiesTransform = instance.transform.Find("Enemies");

            if (enemiesTransform != null)
            {
                // Recorrer cada GameObject hijo de "Enemies"
                foreach (Transform child in enemiesTransform)
                {
                    // Buscar el GameObject "PatrolPoints (Clone)" dentro de cada enemigo
                    Transform patrolPoints = child.Find("PatrolPoints(Clone)");
                    if (patrolPoints != null)
                    {
                        // Cambiar el nombre a "PatrolPoints"
                        patrolPoints.name = "PatrolPoints";
                    }
                }
            }
            else
            {
                Debug.LogWarning($"El prefab en {prefabPath} no tiene un hijo 'Enemies'");
            }

            // Aplicar los cambios al prefab original
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);

            // Destruir la instancia temporal
            GameObject.DestroyImmediate(instance);

            // Refrescar el AssetDatabase para asegurar que los cambios se reflejen
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning($"El prefab en la ruta {prefabPath} no se pudo cargar.");
        }
    }

    public static void ModifyEnemy(string prefabPath)
    {
        // Cargar el prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab != null)
        {
            // Instanciar el prefab para modificarlo
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            // Buscar el GameObject hijo llamado "Enemies"
            Transform enemiesTransform = instance.transform.Find("Enemies");

            if (enemiesTransform != null)
            {
                // Rutas de los nuevos prefabs enemigos
                string[] enemyPrefabs = {
                    "Assets/Project/Prefabs/Enemies/AxeEnemy.prefab",
                    "Assets/Project/Prefabs/Enemies/HammerEnemy.prefab"
                };

                // Recorrer cada GameObject hijo de "Enemies"
                foreach (Transform child in enemiesTransform)
                {
                    if (child.name == "BasicEnemy_v2" || child.name == "BasicEnemy_v2 (1)" || child.name == "BasicEnemy_v2 (2)" || child.name == "BasicEnemy_v2 (3)")
                    {
                        // Buscar el GameObject llamado "PatrolPoints" dentro del BasicEnemy
                        Transform patrolPoints = child.Find("PatrolPoints");
                        if (patrolPoints != null)
                        {
                            // Instanciar un clon de "PatrolPoints" para usarlo luego
                            GameObject patrolPointsClone = GameObject.Instantiate(patrolPoints.gameObject);

                            // Elegir aleatoriamente entre los dos prefabs de enemigo
                            string newEnemyPrefabPath = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                            GameObject newEnemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newEnemyPrefabPath);

                            if (newEnemyPrefab != null)
                            {
                                // Crear una instancia del nuevo enemigo en la misma posición y rotación
                                GameObject newEnemyInstance = (GameObject)PrefabUtility.InstantiatePrefab(newEnemyPrefab, enemiesTransform);
                                newEnemyInstance.transform.position = child.position;
                                newEnemyInstance.transform.rotation = child.rotation;

                                // Colocar "PatrolPoints" como hijo del nuevo enemigo
                                patrolPointsClone.transform.SetParent(newEnemyInstance.transform, worldPositionStays: false);

                                // Destruir el "BasicEnemy" original
                                GameObject.DestroyImmediate(child.gameObject);
                            }
                            else
                            {
                                Debug.LogWarning($"No se pudo cargar el prefab en la ruta {newEnemyPrefabPath}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"El BasicEnemy en {prefabPath} no tiene un hijo 'PatrolPoints'");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"El prefab en {prefabPath} no tiene un hijo 'Enemies'");
            }

            // Aplicar los cambios al prefab original
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);

            // Destruir la instancia temporal
            GameObject.DestroyImmediate(instance);

            // Refrescar el AssetDatabase para asegurar que los cambios se reflejen
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning($"El prefab en la ruta {prefabPath} no se pudo cargar.");
        }
    }
}
