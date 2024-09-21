using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomData))]
public class RoomDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomData roomData = (RoomData)target;

        if (roomData.GetOriginalShape() != null)
        {
            GUILayout.Label("Room Shape:");
            BoolMatrix shape = roomData.GetOriginalShape();
            BoolMatrix entrances = roomData.GetOriginalEntrances();
            int rows = shape.GetLength(1);
            int cols = shape.GetLength(0);

            for (int y = rows - 1; y >= 0; y--)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < cols; x++)
                {
                    shape.SetValue(x, y, GUILayout.Toggle(shape.GetValue(x, y), "[" + y + "," + x + "]"));
                    if (shape.GetValue(x, y) == false)
                    {
                        shape.SetValue(x, y, false);
                    }
                    
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Room Entrances:");
            for (int y = rows - 1; y >= 0; y--)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < cols; x++)
                {
                    EditorGUI.BeginDisabledGroup(!shape.GetValue(x, y));
                    entrances.SetValue(x, y, GUILayout.Toggle(entrances.GetValue(x, y), "[" + y + "," + x + "]"));
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
            }

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (entrances.GetValue(x, y))
                    {
                        GUILayout.Space(10);
                        GUILayout.Label("[" + y + "," + x + "] Directions:");

                        Vector2Int pos = new Vector2Int(x, y);
                        if (!roomData.entrancesDirections.ContainsKey(pos))
                        {
                            roomData.entrancesDirections[pos] = new DirectionFlag();
                        }

                        DirectionFlag direction = roomData.entrancesDirections[pos];

                        GUILayout.BeginHorizontal();
                        
                        bool isUp = false, isDown = false, isRight = false, isLeft = false;
                        if(direction == DirectionFlag.Up)
                        {
                            isUp = true;
                        }
                        if (direction == DirectionFlag.Down)
                        {
                            isDown = true;
                        }
                        if (direction == DirectionFlag.Right)
                        {
                            isRight = true;
                        }
                        if (direction == DirectionFlag.Left)
                        {
                            isLeft = true;
                        }

                        if(GUILayout.Toggle(isUp, "Up"))
                        {
                            roomData.entrancesDirections[pos] = DirectionFlag.Up;
                        }
                        if (GUILayout.Toggle(isDown, "Down"))
                        {
                            roomData.entrancesDirections[pos] = DirectionFlag.Down;
                        }
                        if (GUILayout.Toggle(isRight, "Right"))
                        {
                            roomData.entrancesDirections[pos] = DirectionFlag.Right;
                        }
                        if (GUILayout.Toggle(isLeft, "Left"))
                        {
                            roomData.entrancesDirections[pos] = DirectionFlag.Left;
                        }
                        //direction.Up = GUILayout.Toggle(isUp, "Up");
                        //direction.Down = GUILayout.Toggle(isDown, "Down");
                        //direction.Right = GUILayout.Toggle(isRight, "Right");
                        //direction.Left = GUILayout.Toggle(isLeft, "Left");
                        GUILayout.EndHorizontal();
                    }
                }
            }

        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(roomData);
            AssetDatabase.SaveAssets();
            Repaint();
        }
    }
}
