using System;
using UnityEngine;

namespace CircleFinding
{
    public class CircleFinding : MonoBehaviour
    {

        [SerializeField] private float pointX;
        [SerializeField] private float pointY;

        //Here we think that our functions are tangents
        private float Tangent1(float xVal)
        {
            return 1f / 3f * xVal;
        }

        private float Tangent2(float xVal)
        {
            return 4f / 3f * xVal;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Point point = new Point(pointX, pointY);
                Circle foundCircle = FindCircle(Tangent1, Tangent2, point);
                
                if (foundCircle == null)
                {
                    //Special case may be specified
                    Debug.Log("Circle not found(special case or parallel lines");
                }
                else
                {
                    Debug.Log(foundCircle.ToString());
                }

            }

        }


        public Circle FindCircle(Func<float, float> firstTangent, Func<float, float> secondTangent, Point coord)
        {
            if (firstTangent == null || secondTangent == null || coord == null)
                return null;

            LinearFunctionCoefficients firstTangentCoefficients = FindLinearFunctionCoefficients(firstTangent);
            LinearFunctionCoefficients secondTangentCoefficients = FindLinearFunctionCoefficients(secondTangent);

            Point tangentsCrossPoint = FindLinearFunctionsCrossPoint(firstTangentCoefficients, secondTangentCoefficients);
            if (tangentsCrossPoint == null)
            {
                Debug.Log("Your tangents are parallel");
                return null;
            }

            LinearFunctionCoefficients bisectorLinearCoefficients = FindLinearFunctionsBisector(firstTangentCoefficients,
                secondTangentCoefficients, tangentsCrossPoint);

            //Can be done in another function
            float bisectorASquare = bisectorLinearCoefficients.a * bisectorLinearCoefficients.a;
            
            float divideVal = firstTangentCoefficients.a * firstTangentCoefficients.a + 1f;

            float ar = bisectorASquare + 1;
            float br = 2 * bisectorLinearCoefficients.a * bisectorLinearCoefficients.b - 2 * coord.x - 2 * coord.y * bisectorLinearCoefficients.a;
            float cr = coord.x * coord.x + bisectorLinearCoefficients.b * bisectorLinearCoefficients.b -
                      2 * coord.y * bisectorLinearCoefficients.b + coord.y * coord.y;

            float al = (firstTangentCoefficients.a * firstTangentCoefficients.a + bisectorASquare
                        - 2 * firstTangentCoefficients.a * bisectorLinearCoefficients.a) / divideVal;
            float bl = 2 * (bisectorLinearCoefficients.a * bisectorLinearCoefficients.b - firstTangentCoefficients.a * bisectorLinearCoefficients.b +
                            firstTangentCoefficients.a * firstTangentCoefficients.b - firstTangentCoefficients.b * bisectorLinearCoefficients.a) / divideVal;
            float cl = (bisectorLinearCoefficients.b * bisectorLinearCoefficients.b + firstTangentCoefficients.b * firstTangentCoefficients.b -
                        2 * firstTangentCoefficients.b * bisectorLinearCoefficients.b) / divideVal;

            float a = ar - al;
            float b = br - bl;
            float c = cr - cl;
            
            // it's parabolic function, has two answers, so i get only the highest
            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0f)
            {
                Debug.Log("There are no answers");
                return null;
            }

            float centerX = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
            float centerY = bisectorLinearCoefficients.a * centerX + bisectorLinearCoefficients.b;

            float length = Mathf.Abs(firstTangentCoefficients.a * centerX - centerY + firstTangentCoefficients.b);
            float sqrt = Mathf.Sqrt(divideVal);
            float radius = length / sqrt;

            return new Circle(new Point(centerX, centerY), radius);
        }

        private LinearFunctionCoefficients FindLinearFunctionCoefficients(Func<float, float> linearFunction)
        {
            if (linearFunction == null)
                return null;

            float a = linearFunction.Invoke(1f) - linearFunction.Invoke(0f);
            float b = linearFunction.Invoke(0f);

            return new LinearFunctionCoefficients(a, b);
        }

        private Point FindLinearFunctionsCrossPoint(LinearFunctionCoefficients firstCoefficients, LinearFunctionCoefficients secondCoefficients)
        {
            if (firstCoefficients.a - secondCoefficients.a == 0f)
                return null;

            float x = (secondCoefficients.b - firstCoefficients.b) / (firstCoefficients.a - secondCoefficients.a);
            float y = firstCoefficients.a * x + firstCoefficients.b;
            return new Point(x, y);
        }

        private LinearFunctionCoefficients FindLinearFunctionsBisector(LinearFunctionCoefficients firstCoefficients,
            LinearFunctionCoefficients secondCoefficients, Point crossPoint)
        {
            float tangentCoefficientsMultiple = firstCoefficients.a * secondCoefficients.a;
            if (Math.Abs(tangentCoefficientsMultiple - 1f) < 0.001f)
            {
                //Specific case : tg is undefined
                return null;
            }

            float sumTangentCoefficient = (firstCoefficients.a + secondCoefficients.a) / (1f - tangentCoefficientsMultiple);
            float sumAngle = Mathf.Atan(sumTangentCoefficient);
            float bisectorTangentCoefficient = (1f - Mathf.Cos(sumAngle)) / Mathf.Sin(sumAngle);

            float b = crossPoint.y - bisectorTangentCoefficient * crossPoint.x;

            return new LinearFunctionCoefficients(bisectorTangentCoefficient, b);
        }
        
        
    }
}
