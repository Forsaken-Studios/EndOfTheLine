using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomData))]
public class RoomDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomData roomData = (RoomData)target;

        // Forzar el tamaño fijo de roomSize
        roomData.roomSize = new Vector2Int(3, 3);

        // Forzar _shape a ser 3x3 con todo en true
        if (roomData.GetOriginalShape() == null ||
            roomData.GetOriginalShape().GetLength(0) != 3 ||
            roomData.GetOriginalShape().GetLength(1) != 3)
        {
            roomData.OnValidate(); // Asegurar inicialización correcta
            BoolMatrix shape = roomData.GetOriginalShape();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    shape.SetValue(x, y, true); // Asegurar que todas las celdas están en true
                }
            }
        }

        EditorGUILayout.LabelField("Entradas", EditorStyles.boldLabel);

        // Mostrar la matriz con ajuste de espaciado horizontal entre elementos
        BoolMatrix entrances = roomData.GetOriginalEntrances();

        for (int visualY = 2; visualY >= 0; visualY--) // Iterar sobre filas para renderizar en orden visual
        {
            EditorGUILayout.BeginHorizontal(); // Nueva fila
            for (int x = 0; x < 3; x++) // Iterar sobre columnas
            {
                EditorGUILayout.LabelField($"({visualY},{x})", GUILayout.Width(50)); // Texto con ancho fijo
                bool currentState = entrances.GetValue(x, visualY);
                bool newState = EditorGUILayout.Toggle(currentState, GUILayout.Width(20)); // Toggle con ancho fijo
                entrances.SetValue(x, visualY, newState); // Actualizar el valor en la matriz

                if (newState)
                {
                    Vector2Int position = new Vector2Int(x, visualY);
                    DirectionFlag flag = roomData.entrancesDirections.TryGetValue(position, out var existingFlag)
                        ? existingFlag
                        : DirectionFlag.None;
                    roomData.entrancesDirections[position] = (DirectionFlag)EditorGUILayout.EnumPopup(flag, GUILayout.Width(60));
                }

                GUILayout.Space(80); // Añadir más espacio entre columnas
            }
            EditorGUILayout.EndHorizontal(); // Termina la fila
        }

        // Aplicar cambios si el objeto fue modificado
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
