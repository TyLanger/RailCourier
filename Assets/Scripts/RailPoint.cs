using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPoint : MonoBehaviour
{

    public RailPoint nextPoint;
    public RailPoint altPoint;

    public bool useAlt = false;

    [SerializeField] int carsInArea = 0;
    protected List<Car> cars;

    LineRenderer line;
    LineRenderer lr2;

    private void Start()
    {
        cars = new List<Car>();
        line = GetComponent<LineRenderer>();

        int resolution = 2;

        if(altPoint != null)
        {
            GameObject g = new GameObject(gameObject.name + " 2nd LR");
            g.transform.position = transform.position;
            g.transform.parent = transform;

            lr2 = g.AddComponent<LineRenderer>();
            lr2.textureMode = LineTextureMode.Tile;
            lr2.material = line.material;
            lr2.positionCount = resolution;
            lr2.SetPosition(0, altPoint.transform.position);
            lr2.SetPosition(1, transform.position);

        }

        line.positionCount = resolution;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, nextPoint.transform.position);


    }

    public RailPoint GetNext(Car car)
    {
        Exit(car);

        // maybe trains can't use alt rails?
        if(useAlt && car.GetType() == typeof(Train))
        {
            Switch();
        }
        if (useAlt && altPoint != null)
        {
            altPoint.Enter(car);
            return altPoint;
        }
        nextPoint.Enter(car);
        return nextPoint;
    }

    public void Switch()
    {
        useAlt = !useAlt;
    }

    public virtual void Enter(Car car)
    {
        carsInArea++;
        cars.Add(car);
    }

    public virtual void Exit(Car car)
    {
        car.Highlight(false);

        carsInArea--;
        cars.Remove(car);
    }

    public bool DoesCarHaveBehind()
    {
        if(cars.Count> 0 && cars[cars.Count-1].hasBehind)
        {
            return true;
        }
        return false;
    }
}
