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

    private void Start()
    {
        cars = new List<Car>();
    }

    public RailPoint GetNext(Car car)
    {
        if(useAlt && altPoint != null)
        {
            return altPoint;
        }
        Exit(car);
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
}
