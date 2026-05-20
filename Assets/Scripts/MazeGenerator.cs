using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("迷路の設定")]
    [Tooltip("通路の数。16に設定すると1辺のブロック数は33になります")]
    public int pathCount = 16;

    [Header("各要素の高さ")]
    public float outerWallHeight = 2f; // 外壁の高さ
    public float innerWallHeight = 2f; // 内壁の高さ
    public float pathHeight = 1f;      // 通路の高さ

    [Header("取得用座標（他スクリプトから参照可能）")]
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

        // 迷路配列初期化
        for (int x = 0; x < logicalSize; x++)
        {
            for (int z = 0; z < logicalSize; z++)
            {
                maze[x, z] = 0;
            }
        }

        // DFS用スタック
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        // スタート地点
        Vector2Int startPos = new Vector2Int(1, 1);

        maze[startPos.x, startPos.y] = 1;

        stack.Push(startPos);

        // 移動方向
        Vector2Int[] dirs =
        {
            new Vector2Int(0, 2),
            new Vector2Int(0, -2),
            new Vector2Int(2, 0),
            new Vector2Int(-2, 0)
        };

        // 深さ優先探索
        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();

            List<Vector2Int> validDirs =
                new List<Vector2Int>();

            foreach (var dir in dirs)
            {
                int nx = current.x + dir.x;
                int nz = current.y + dir.y;

                if (nx > 0 &&
                    nx < logicalSize - 1 &&
                    nz > 0 &&
                    nz < logicalSize - 1)
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
                    validDirs[
                        UnityEngine.Random.Range(
                            0,
                            validDirs.Count
                        )
                    ];

                // 間の壁を壊す
                maze[
                    current.x + chosenDir.x / 2,
                    current.y + chosenDir.y / 2
                ] = 1;

                // 次の通路
                maze[
                    current.x + chosenDir.x,
                    current.y + chosenDir.y
                ] = 1;

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

        // --- Unity空間へ配置 ---
        float[] positions = new float[logicalSize];

        float currentPos = 0f;

        for (int i = 0; i < logicalSize; i++)
        {
            float size =
                (i % 2 == 0) ? 1f : 2f;

            positions[i] =
                currentPos + size / 2f;

            currentPos += size;
        }

        for (int x = 0; x < logicalSize; x++)
        {
            for (int z = 0; z < logicalSize; z++)
            {
                float sizeX =
                    (x % 2 == 0) ? 1f : 2f;

                float sizeZ =
                    (z % 2 == 0) ? 1f : 2f;

                float posX = positions[x];
                float posZ = positions[z];

                // 壁の場合
                if (maze[x, z] == 0)
                {
                    bool isOuter =
                        (x == 0 ||
                         x == logicalSize - 1 ||
                         z == 0 ||
                         z == logicalSize - 1);

                    float height =
                        isOuter
                        ? outerWallHeight
                        : innerWallHeight;

                    GameObject wall =
                        GameObject.CreatePrimitive(
                            PrimitiveType.Cube
                        );

                    wall.transform.parent =
                        this.transform;

                    // 壁を地面から上方向へ配置
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
                        isOuter
                        ? "OuterWall"
                        : "InnerWall";

                    wall.GetComponent<Renderer>()
                        .material.color =
                        isOuter
                        ? Color.black
                        : Color.gray;
                }
                // 通路・スタート・ゴール
                else
                {
                    GameObject path =
                        GameObject.CreatePrimitive(
                            PrimitiveType.Cube
                        );

                    path.transform.parent =
                        this.transform;

                    // 通路を地面として下方向へ配置
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

                    // スタート地点
                    if (x == 1 && z == 1)
                    {
                        path.name = "StartCube";

                        path.GetComponent<Renderer>()
                            .material.color =
                            Color.blue;

                        startWorldPosition =
                            this.transform.TransformPoint(
                                new Vector3(
                                    posX,
                                    0f,
                                    posZ
                                )
                            );
                    }
                    // ゴール地点
                    else if (
                        x == logicalSize - 2 &&
                        z == logicalSize - 2)
                    {
                        path.name = "GoalCube";

                        path.GetComponent<Renderer>()
                            .material.color =
                            Color.red;

                        goalWorldPosition =
                            this.transform.TransformPoint(
                                new Vector3(
                                    posX,
                                    0f,
                                    posZ
                                )
                            );
                    }
                    // 通常通路
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