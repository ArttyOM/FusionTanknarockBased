using UnityEngine;

namespace FusionExamples.Tanknarok
{
    public class NetworkInputDataAdapter
    {
        public void ConvertAimDirectionToNetworkInput(in Vector2 aimDirection, 
            out sbyte networkInputAimDirectionX, out sbyte networkInputAimDirectionY)
        {
            ConvertVector2ToSbyteXY(in aimDirection,
                out sbyte networkInputDirectionX, out sbyte networkInputDirectionY);
            
            networkInputAimDirectionX = networkInputDirectionX;
            networkInputAimDirectionY = networkInputDirectionY;
        }
        
        public void ConvertMoveDirectionToNetworkInput(in Vector2 moveDirection, 
            out sbyte networkInputMoveDirectionX, out sbyte networkInputMoveDirectionY)
        {
            ConvertVector2ToSbyteXY(in moveDirection,
                out sbyte networkInputDirectionX, out sbyte networkInputDirectionY);
            
            networkInputMoveDirectionX = networkInputDirectionX;
            networkInputMoveDirectionY = networkInputDirectionY;
        }
        
        public Vector2 GetAimDirection(NetworkInputData networkInput)
        {
            return ConvertSbyteXYToVector2(networkInput.aimDirectionX, networkInput.aimDirectionY);
        }

        public Vector2 GetMoveDirection(NetworkInputData networkInput)
        {
            return ConvertSbyteXYToVector2(networkInput.moveDirectionX, networkInput.moveDirectionY);
        }

        private Vector2 ConvertSbyteXYToVector2(in sbyte networkInputDirectionX, in sbyte networkInputDirectionY)
        {
            Vector2 result;
            result.x = ((float) networkInputDirectionX) / sbyte.MaxValue;
            result.y = ((float) networkInputDirectionY) / sbyte.MaxValue;
            return result;
        }
        
        private void ConvertVector2ToSbyteXY(in Vector2 direction, 
            out sbyte networkInputDirectionX, out sbyte networkInputDirectionY)
        {
            Vector2 normalizedAimDirection = direction.normalized;
            networkInputDirectionX = (sbyte) (normalizedAimDirection.x * sbyte.MaxValue);
            networkInputDirectionY = (sbyte) (normalizedAimDirection.y * sbyte.MaxValue);
        }
    }
}