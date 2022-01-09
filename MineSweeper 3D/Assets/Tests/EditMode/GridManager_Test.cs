using NUnit.Framework;
using UnityEngine;

public class GridManager_Test
{
    [Test]
    public void CalcBorderScale_Test()
    {
        var gridObj = new GameObject();
        var gridManager = gridObj.AddComponent<GridManager>();
        const int difficulty1 = 1;
        const int difficulty2 = 2;
        const int difficulty3 = 3;
        Vector3 expectedResult1 = new Vector3(difficulty1, 1, difficulty1);
        Vector3 actualResult1 = gridManager.CalcBorderScale(difficulty1);
        Assert.AreEqual(expectedResult1, actualResult1);

        Vector3 expectedResult2 = new Vector3(difficulty2, 1, difficulty2);
        Vector3 actualResult2 = gridManager.CalcBorderScale(difficulty2);
        Assert.AreEqual(expectedResult2, actualResult2);

        Vector3 expectedResult3 = new Vector3(difficulty3, 1, difficulty3);
        Vector3 actualResult3 = gridManager.CalcBorderScale(difficulty3);
        Assert.AreEqual(expectedResult3, actualResult3);

    }
    
    [Test]
    public void CalcBorderPos_Test()
    {
        var gridObj = new GameObject();
        var gridManager = gridObj.AddComponent<GridManager>();
        const int gridSize1 = 9;
        const int gridSize2 = 17;
        const int gridSize3 = 25;
        const int tileSize = 5;

        Vector3 expectedResult1 = new Vector3(gridSize1 * tileSize, 0, gridSize1 * tileSize);
        Vector3 actualResult1 = gridManager.CalcBorderPos(gridSize1, tileSize);
        Assert.AreEqual(expectedResult1, actualResult1);

        Vector3 expectedResult2 = new Vector3(gridSize2 * tileSize, 0, gridSize2 * tileSize);
        Vector3 actualResult2 = gridManager.CalcBorderPos(gridSize2, tileSize);
        Assert.AreEqual(expectedResult2, actualResult2);

        Vector3 expectedResult3 = new Vector3(gridSize3 * tileSize, 0, gridSize3 * tileSize);
        Vector3 actualResult3 = gridManager.CalcBorderPos(gridSize3, tileSize);
        Assert.AreEqual(expectedResult3, actualResult3);
    }
}
