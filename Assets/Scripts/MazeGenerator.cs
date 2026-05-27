using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("迷路の設定")]
    [Tooltip("通路の数。16に設定すると1辺の物理的な長さは49になります")]
    public int pathCount = 16;

    [Header("各要素の高さ")]
    public float outerWallHeight = 2f; // 外壁の高さ
    public float innerWallHeight = 2f; // 内壁の高さ
    public float pathHeight = 1f;      // 通路の高さ

    [Header("取得用（他のスクリプトから参照可能）")]
    public Vector3 startWorldPosition;
    public Vector3 goalWorldPosition;

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        int logicalSize = pathCount * 2 + 1;
        int[,] maze = new int[logicalSize, logicalSize];

        for (int x = 0; x < logicalSize; x++)
            for (int z = 0; z < logicalSize; z++)
                maze[x, z] = 0;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int startPos = new Vector2Int(1, 1);
        maze[startPos.x, startPos.y] = 1;
        stack.Push(startPos);

        Vector2Int[] dirs =
        {
            new Vector2Int(0, 2),
            new Vector2Int(0, -2),
            new Vector2Int(2, 0),
            new Vector2Int(-2, 0)
        };

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> validDirs = new List<Vector2Int>();

            foreach (var dir in dirs)
            {
                int nx = current.x + dir.x;
                int nz = current.y + dir.y;

                if (nx > 0 && nx < logicalSize - 1 &&
                    nz > 0 && nz < logicalSize - 1)
                {
                    if (maze[nx, nz] == 0)
                    {
                        validDirs.Add(dir);
                    }
                }
            }

            if (validDirs.Count > 0)
            {
                Vector2Int chosenDir =
                    validDirs[UnityEngine.Random.Range(0, validDirs.Count)];

                maze[current.x + chosenDir.x / 2,
                     current.y + chosenDir.y / 2] = 1;

                maze[current.x + chosenDir.x,
                     current.y + chosenDir.y] = 1;

                stack.Push(
                    new Vector2Int(
                        current.x + chosenDir.x,
                        current.y + chosenDir.y
                    )
                );
            }
            else
            {
                stack.Pop();
            }
        }

        // --- 実際のUnity空間への配置 ---
        float[] positions = new float[logicalSize];
        float currentPos = 0f;

        for (int i = 0; i < logicalSize; i++)
        {
            float size = (i % 2 == 0) ? 1f : 2f;
            positions[i] = currentPos + size / 2f;
            currentPos += size;
        }

        for (int x = 0; x < logicalSize; x++)
        {
            for (int z = 0; z < logicalSize; z++)
            {
                float sizeX = (x % 2 == 0) ? 1f : 2f;
                float sizeZ = (z % 2 == 0) ? 1f : 2f;
                float posX = positions[x];
                float posZ = positions[z];

                if (maze[x, z] == 0) // 壁の場合
                {
                    bool isOuter =
                        (x == 0 || x == logicalSize - 1 ||
                         z == 0 || z == logicalSize - 1);

                    float height =
                        isOuter ? outerWallHeight : innerWallHeight;

                    GameObject wall =
                        GameObject.CreatePrimitive(
                            PrimitiveType.Cube
                        );

                    wall.transform.parent = this.transform;

                    // 壁をY=0の地面から上方向に見えるよう配置
                    wall.transform.localPosition =
                        new Vector3(
                            posX,
                            height / 2f,
                            posZ
                        );

                    wall.transform.localScale =
                        new Vector3(
                            sizeX,
                            height,
                            sizeZ
                        );

                    wall.name =
                        isOuter ? "OuterWall" : "InnerWall";

                    wall.GetComponent<Renderer>()
                        .material.color =
                        isOuter ? Color.black : Color.gray;
                }
                else // 通路・スタート・ゴールの場合
                {
                    GameObject path =
                        GameObject.CreatePrimitive(
                            PrimitiveType.Cube
                        );

                    path.transform.parent = this.transform;

                    // 通路をY=0を表面として下方向に配置
                    path.transform.localPosition =
                        new Vector3(
                            posX,
                            -pathHeight / 2f,
                            posZ
                        );

                    path.transform.localScale =
                        new Vector3(
                            sizeX,
                            pathHeight,
                            sizeZ
                        );

                    if (x == 1 && z == 1) // スタート地点
                    {
                        path.name = "StartCube";
                        path.GetComponent<Renderer>()
                            .material.color = Color.blue;

                        startWorldPosition =
                            this.transform.TransformPoint(
                                new Vector3(
                                    posX,
                                    0f,
                                    posZ
                                )
                            );
                    }
                    else if (
                        x == logicalSize - 2 &&
                        z == logicalSize - 2
                    ) // ゴール地点
                    {
                        path.name = "GoalCube";
                        path.GetComponent<Renderer>()
                            .material.color = Color.red;

                        goalWorldPosition =
                            this.transform.TransformPoint(
                                new Vector3(
                                    posX,
                                    0f,
                                    posZ
                                )
                            );
                    }
                    else
                    {
                        path.name = "Path";
                        path.GetComponent<Renderer>()
                            .material.color =
                            new Color(
                                0.6f,
                                0.9f,
                                0.6f
                            );
                    }
                }
            }
        }
    }
}