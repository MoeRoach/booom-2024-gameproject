// File create date:8/15/2018
using System;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 3D环境中常用的数学方法
    /// </summary>
    public class MathFor3D {
        //increase or decrease the length of vector by size
        public static Vector3 AddVectorLength(Vector3 vector, float size) {
            //get the vector length
            var magnitude = Vector3.Magnitude(vector);
            //change the length
            magnitude += size;
            //normalize the vector
            var vectorNormalized = Vector3.Normalize(vector);
            //scale the vector
            return Vector3.Scale(vectorNormalized,
                new Vector3(magnitude, magnitude, magnitude));
        }

        //create a vector of direction "vector" with length "size"
        public static Vector3 SetVectorLength(Vector3 vector, float size) {
            //normalize the vector
            var vectorNormalized = Vector3.Normalize(vector);
            //scale the vector
            return vectorNormalized *= size;
        }

        //caclulate the rotational difference from A to B
        public static Quaternion SubtractRotation(Quaternion b, Quaternion a) {
            var c = Quaternion.Inverse(a) * b;
            return c;
        }

        //Find the line of intersection between two planes. The planes are defined by a normal and a point on that plane.
        //The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
        //the function outputs true, otherwise false.
        public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec,
            Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal,
            Vector3 plane2Position) {
            linePoint = Vector3.zero;
            lineVec = Vector3.zero;
            //We can get the direction of the line of intersection of the two planes by calculating the 
            //cross product of the normals of the two planes. Note that this is just a direction and the line
            //is not fixed in space yet. We need a point for that to go with the line vector.
            lineVec = Vector3.Cross(plane1Normal, plane2Normal);
            //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
            //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
            //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
            //the cross product of the normal of plane2 and the lineDirection.      
            var ldir = Vector3.Cross(plane2Normal, lineVec);
            var denominator = Vector3.Dot(plane1Normal, ldir);
            //Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
            if (!(Mathf.Abs(denominator) > 0.006f)) return false;
            var plane1ToPlane2 = plane1Position - plane2Position;
            var t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
            linePoint = plane2Position + t * ldir;
            return true;

        }

        //Get the intersection between a line and a plane. 
        //If the line and plane are not parallel, the function outputs true, otherwise false.
        public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint,
            Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint) {
            intersection = Vector3.zero;
            //calculate the distance between the linePoint and the line-plane intersection point
            var dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            var dotDenominator = Vector3.Dot(lineVec, planeNormal);
            //line and plane are not parallel
            if (Mathf.Approximately(dotDenominator, 0f)) return false;
            var length = dotNumerator / dotDenominator;
            //create a vector from the linePoint to the intersection point
            var vector = SetVectorLength(lineVec, length);
            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;
            return true;

        }

        //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
        //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
        //same plane, use ClosestPointsOnTwoLines() instead.
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
            Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2) {

            intersection = Vector3.zero;

            var lineVec3 = linePoint2 - linePoint1;
            var crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            var crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            var planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //Lines are not coplanar. Take into account rounding errors.
            if ((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f)) {

                return false;
            }

            //Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
            var s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            if ((!(s >= 0.0f)) || (!(s <= 1.0f))) return false;
            intersection = linePoint1 + (lineVec1 * s);
            return true;

        }

        //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        //to each other. This function finds those two points. If the lines are not parallel, the function 
        //outputs true, otherwise false.
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1,
            out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1,
            Vector3 linePoint2, Vector3 lineVec2) {

            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            var a = Vector3.Dot(lineVec1, lineVec1);
            var b = Vector3.Dot(lineVec1, lineVec2);
            var e = Vector3.Dot(lineVec2, lineVec2);
            var d = a * e - b * b;

            //lines are not parallel
            if (Mathf.Approximately(d, 0f)) return false;
            var r = linePoint1 - linePoint2;
            var c = Vector3.Dot(lineVec1, r);
            var f = Vector3.Dot(lineVec2, r);
            var s = (b * f - c * e) / d;
            var t = (a * f - c * b) / d;
            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;
            return true;
        }

        //This function returns a point which is a projection from a point to a line.
        //The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
        public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec,
            Vector3 point) {
            //get vector from point on line to point in space
            var linePointToPoint = point - linePoint;
            var t = Vector3.Dot(linePointToPoint, lineVec);
            return linePoint + lineVec * t;
        }

        //This function returns a point which is a projection from a point to a line segment.
        //If the projected point lies outside of the line segment, the projected point will 
        //be clamped to the appropriate line edge.
        //If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
        public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2,
            Vector3 point) {
            var vector = linePoint2 - linePoint1;
            var projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);
            var side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);
            return side switch {
                //The projected point is on the line segment
                0 => projectedPoint,
                1 => linePoint1,
                2 => linePoint2,
                _ => Vector3.zero
            };
            //output is invalid
        }

        //This function returns a point which is a projection from a point to a plane.
        public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint,
            Vector3 point) {
            //First calculate the distance from the point to the plane:
            var distance = SignedDistancePlanePoint(planeNormal, planePoint, point);
            //Reverse the sign of the distance
            distance *= -1;
            //Get a translation vector
            var translationVector = SetVectorLength(planeNormal, distance);
            //Translate the point to form a projection
            return point + translationVector;
        }

        //Projects a vector onto a plane. The output is not normalized.
        public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector) {
            return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
        }

        //Get the shortest distance between a point and a plane. The output is signed so it holds information
        //as to which side of the plane normal the point is.
        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint,
            Vector3 point) {
            return Vector3.Dot(planeNormal, (point - planePoint));
        }

        //This function calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
        //to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
        //by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
        //the result of a dot product only has signed information when an angle is transitioning between more or less
        //then 90 degrees.
        public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal) {
            //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
            var perpVector = Vector3.Cross(normal, vectorA);
            //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
            var dot = Vector3.Dot(perpVector, vectorB);
            return dot;
        }

        public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector,
            Vector3 normal) {
            //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
            var perpVector = Vector3.Cross(normal, referenceVector);
            //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
            var angle = Vector3.Angle(referenceVector, otherVector);
            angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));
            return angle;
        }

        //Calculate the angle between a vector and a plane. The plane is made by a normal vector.
        //Output is in radians.
        public static float AngleVectorPlane(Vector3 vector, Vector3 normal) {
            //calculate the the dot product between the two input vectors. This gives the cosine between the two vectors
            var dot = Vector3.Dot(vector, normal);
            //this is in radians
            var angle = (float) Math.Acos(dot);
            return 1.570796326794897f - angle; //90 degrees - angle
        }

        //Calculate the dot product as an angle
        public static float DotProductAngle(Vector3 vec1, Vector3 vec2) {

            double dot;
            double angle;

            //get the dot product
            dot = Vector3.Dot(vec1, vec2);

            //Clamp to prevent NaN error. Shouldn't need this in the first place, but there could be a rounding error issue.
            if (dot < -1.0f) {
                dot = -1.0f;
            }

            if (dot > 1.0f) {
                dot = 1.0f;
            }

            //Calculate the angle. The output is in radians
            //This step can be skipped for optimization...
            angle = Math.Acos(dot);

            return (float) angle;
        }

        //Convert a plane defined by 3 points to a plane defined by a vector and a point. 
        //The plane point is the middle of the triangle defined by the 3 points.
        public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint,
            Vector3 pointA, Vector3 pointB, Vector3 pointC) {

            planeNormal = Vector3.zero;
            planePoint = Vector3.zero;

            //Make two vectors from the 3 input points, originating from point A
            var AB = pointB - pointA;
            var AC = pointC - pointA;

            //Calculate the normal
            planeNormal = Vector3.Normalize(Vector3.Cross(AB, AC));

            //Get the points in the middle AB and AC
            var middleAB = pointA + (AB / 2.0f);
            var middleAC = pointA + (AC / 2.0f);

            //Get vectors from the middle of AB and AC to the point which is not on that line.
            var middleABtoC = pointC - middleAB;
            var middleACtoB = pointB - middleAC;

            //Calculate the intersection between the two lines. This will be the center 
            //of the triangle defined by the 3 points.
            //We could use LineLineIntersection instead of ClosestPointsOnTwoLines but due to rounding errors 
            //this sometimes doesn't work.
            ClosestPointsOnTwoLines(out planePoint, out _, middleAB, middleABtoC, middleAC,
                middleACtoB);
        }

        //Returns the forward vector of a quaternion
        public static Vector3 GetForwardVector(Quaternion q) {

            return q * Vector3.forward;
        }

        //Returns the up vector of a quaternion
        public static Vector3 GetUpVector(Quaternion q) {

            return q * Vector3.up;
        }

        //Returns the right vector of a quaternion
        public static Vector3 GetRightVector(Quaternion q) {

            return q * Vector3.right;
        }

        //Gets a quaternion from a matrix
        public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {

            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }

        //Gets a position from a matrix
        public static Vector3 PositionFromMatrix(Matrix4x4 m) {

            var vector4Position = m.GetColumn(3);
            return new Vector3(vector4Position.x, vector4Position.y, vector4Position.z);
        }

        //This is an alternative for Quaternion.LookRotation. Instead of aligning the forward and up vector of the game 
        //object with the input vectors, a custom direction can be used instead of the fixed forward and up vectors.
        //alignWithVector and alignWithNormal are in world space.
        //customForward and customUp are in object space.
        //Usage: use alignWithVector and alignWithNormal as if you are using the default LookRotation function.
        //Set customForward and customUp to the vectors you wish to use instead of the default forward and up vectors.
        public static void LookRotationExtended(ref GameObject gameObjectInOut,
            Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward,
            Vector3 customUp) {

            //Set the rotation of the destination
            var rotationA = Quaternion.LookRotation(alignWithVector, alignWithNormal);

            //Set the rotation of the custom normal and up vectors. 
            //When using the default LookRotation function, this would be hard coded to the forward and up vector.
            var rotationB = Quaternion.LookRotation(customForward, customUp);

            //Calculate the rotation
            gameObjectInOut.transform.rotation = rotationA * Quaternion.Inverse(rotationB);
        }

        //With this function you can align a triangle of an object with any transform.
        //Usage: gameObjectInOut is the game object you want to transform.
        //alignWithVector, alignWithNormal, and alignWithPosition is the transform with which the triangle of the object should be aligned with.
        //triangleForward, triangleNormal, and trianglePosition is the transform of the triangle from the object.
        //alignWithVector, alignWithNormal, and alignWithPosition are in world space.
        //triangleForward, triangleNormal, and trianglePosition are in object space.
        //trianglePosition is the mesh position of the triangle. The effect of the scale of the object is handled automatically.
        //trianglePosition can be set at any position, it does not have to be at a vertex or in the middle of the triangle.
        public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector,
            Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward,
            Vector3 triangleNormal, Vector3 trianglePosition) {

            //Set the rotation.
            LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal,
                triangleForward, triangleNormal);

            //Get the world space position of trianglePosition
            var trianglePositionWorld =
                gameObjectInOut.transform.TransformPoint(trianglePosition);

            //Get a vector from trianglePosition to alignWithPosition
            var translateVector = alignWithPosition - trianglePositionWorld;

            //Now transform the object so the triangle lines up correctly.
            gameObjectInOut.transform.Translate(translateVector, Space.World);
        }


        //Convert a position, direction, and normal vector to a transform
        public static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector,
            Vector3 directionVector, Vector3 normalVector) {
            gameObjectInOut.transform.position = positionVector;
            gameObjectInOut.transform.rotation =
                Quaternion.LookRotation(directionVector, normalVector);
        }

        //This function finds out on which side of a line segment the point is located.
        //The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
        //the line segment, project it on the line using ProjectPointOnLine() first.
        //Returns 0 if point is on the line segment.
        //Returns 1 if point is outside of the line segment and located on the side of linePoint1.
        //Returns 2 if point is outside of the line segment and located on the side of linePoint2.
        public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2,
            Vector3 point) {
            var lineVec = linePoint2 - linePoint1;
            var pointVec = point - linePoint1;
            var dot = Vector3.Dot(pointVec, lineVec);
            //point is on side of linePoint2, compared to linePoint1
            if (dot > 0) {
                //point is on the line segment
                return pointVec.magnitude <= lineVec.magnitude ? 0 : 2;
            }
            //Point is not on side of linePoint2, compared to linePoint1.
            //Point is not on the line segment and it is on the side of linePoint1.
            return 1;
        }

        //Returns true if a line segment (made up of linePoint1 and linePoint2) is fully or partially in a rectangle
        //made up of RectA to RectD. The line segment is assumed to be on the same plane as the rectangle. If the line is 
        //not on the plane, use ProjectPointOnPlane() on linePoint1 and linePoint2 first.
        public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2,
            Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD) {
            var pointBInside = false;
            var pointAInside = IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);
            if (!pointAInside) {
                pointBInside = IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
            }
            //none of the points are inside, so check if a line is crossing
            if (pointAInside || pointBInside) return true;
            var lineACrossing =
                AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
            var lineBCrossing =
                AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
            var lineCCrossing =
                AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
            var lineDCrossing =
                AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);
            return lineACrossing || lineBCrossing || lineCCrossing || lineDCrossing;

        }

        //Returns true if "point" is in a rectangle mad up of RectA to RectD. The line point is assumed to be on the same 
        //plane as the rectangle. If the point is not on the plane, use ProjectPointOnPlane() first.
        public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC,
            Vector3 rectB, Vector3 rectD) {
            //get the center of the rectangle
            var vector = rectC - rectA;
            var size = -(vector.magnitude / 2f);
            vector = AddVectorLength(vector, size);
            var middle = rectA + vector;
            var xVector = rectB - rectA;
            var width = xVector.magnitude / 2f;
            var yVector = rectD - rectA;
            var height = yVector.magnitude / 2f;
            var linePoint = ProjectPointOnLine(middle, xVector.normalized, point);
            vector = linePoint - point;
            var yDistance = vector.magnitude;
            linePoint = ProjectPointOnLine(middle, yVector.normalized, point);
            vector = linePoint - point;
            var xDistance = vector.magnitude;
            return (xDistance <= width) && (yDistance <= height);
        }

        //Returns true if line segment made up of pointA1 and pointA2 is crossing line segment made up of
        //pointB1 and pointB2. The two lines are assumed to be in the same plane.
        public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2,
            Vector3 pointB1, Vector3 pointB2) {
            var lineVecA = pointA2 - pointA1;
            var lineVecB = pointB2 - pointB1;
            var valid = ClosestPointsOnTwoLines(out var closestPointA, out var closestPointB, pointA1,
                lineVecA.normalized, pointB1, lineVecB.normalized);
            //lines are not parallel
            if (!valid) return false;
            var sideA = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointA);
            var sideB = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointB);
            return (sideA == 0) && (sideB == 0);
            //lines are parallel
        }
    }
}

