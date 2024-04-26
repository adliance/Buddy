using System.Linq;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;

namespace Adliance.Buddy.QrCode;

public class Squircle(
    Vector2 location,
    float height,
    float width,
    float cornerFactor = 0.98f,
    float straightsFactor = 0.99f) : Polygon(CreateSegment(location, height, width, cornerFactor, straightsFactor))
{
    public Squircle(float height, float width) : this(Vector2.Zero, height, width)
    {
    }

    public Squircle(float x, float y, float height, float width) : this(new Vector2(x, y), height, width)
    {
    }

    private static CubicBezierLineSegment CreateSegment(Vector2 location, float height, float width, float cornerFactor, float straightsFactor)
    {
        var inverseCornerFactor = 1 - cornerFactor;
        var inverseStraightsFactor = 1 - straightsFactor;

        var points = new PointF[]
        {
            // top center
            new(width / 2, 0f),
            // handle to the right of top center
            new Vector2(width * cornerFactor, height * inverseStraightsFactor),
            // handle above the right center
            new Vector2(width * straightsFactor, height * inverseCornerFactor),
            // right center
            new Vector2(width, height / 2),
            // handle below the right center
            new Vector2(width * straightsFactor, height * cornerFactor),
            // handle to the right of bottom center
            new Vector2(width * cornerFactor, height * straightsFactor),
            // bottom center
            new Vector2(width / 2, height),
            // handle to the left of bottom center
            new Vector2(width * inverseCornerFactor, height * straightsFactor),
            // handle below the left center
            new Vector2(width * inverseStraightsFactor, height * cornerFactor),
            // left center
            new Vector2(0f, height / 2),
            // handle above the left center
            new Vector2(width * inverseStraightsFactor, height * inverseCornerFactor),
            // handle to the left of top center
            new Vector2(width * inverseCornerFactor, height * inverseStraightsFactor),
            // top center
            new Vector2(width / 2, 0f)
        };

        points = points.Select(x => PointF.Add(x, location)).ToArray();


        return new CubicBezierLineSegment(points);
    }
}