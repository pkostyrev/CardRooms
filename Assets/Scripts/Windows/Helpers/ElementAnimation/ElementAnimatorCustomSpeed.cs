
using UnityEngine;

namespace CardRooms.Windows.Helpers.ElementAnimation
{
    public class ElementAnimatorCustomSpeed : MonoBehaviour
    {
        internal float AnimationSpeed => animationSpeed;

        [SerializeField] private float animationSpeed;
    }
}
