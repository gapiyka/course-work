using NUnit.Framework;
using UnityEngine;

public class GridManager_Test
{
    [Test]
    public void CalcBorderScale_Test()
    {
        var gridObj = new GameObject();
        var gridManager = gridObj.AddComponent<GridManager>();
        int parameter1 = 1;
        int parameter2 = 2;
        int parameter3 = 3;
        Vector3 expectedResult1 = new Vector3(parameter1, 1, parameter1);
        Vector3 actualResult1 = gridManager.CalcBorderScale(parameter1);
        Assert.AreEqual(expectedResult1, actualResult1);

        Vector3 expectedResult2 = new Vector3(parameter2, 1, parameter2);
        Vector3 actualResult2 = gridManager.CalcBorderScale(parameter2);
        Assert.AreEqual(expectedResult2, actualResult2);

        Vector3 expectedResult3 = new Vector3(parameter3, 1, parameter3);
        Vector3 actualResult3 = gridManager.CalcBorderScale(parameter3);
        Assert.AreEqual(expectedResult3, actualResult3);

    }
}
