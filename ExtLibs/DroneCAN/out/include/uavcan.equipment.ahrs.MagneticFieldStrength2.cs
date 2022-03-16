

using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;

using float32 = System.Single;

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DroneCAN
{
    public partial class DroneCAN {




        public const int UAVCAN_EQUIPMENT_AHRS_MAGNETICFIELDSTRENGTH2_MAX_PACK_SIZE = 26;
        public const ulong UAVCAN_EQUIPMENT_AHRS_MAGNETICFIELDSTRENGTH2_DT_SIG = 0xB6AC0C442430297E;

        public const int UAVCAN_EQUIPMENT_AHRS_MAGNETICFIELDSTRENGTH2_DT_ID = 1002;






        public partial class uavcan_equipment_ahrs_MagneticFieldStrength2: IDroneCANSerialize {



            public uint8_t sensor_id = new uint8_t();



            [MarshalAs(UnmanagedType.ByValArray,SizeConst=3)] public Single[] magnetic_field_ga = new Single[3];



            public uint8_t magnetic_field_covariance_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=9)] public Single[] magnetic_field_covariance = Enumerable.Range(1, 9).Select(i => new Single()).ToArray();




            public void encode(dronecan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
            {
                encode_uavcan_equipment_ahrs_MagneticFieldStrength2(this, chunk_cb, ctx);
            }

            public void decode(CanardRxTransfer transfer)
            {
                decode_uavcan_equipment_ahrs_MagneticFieldStrength2(transfer, this);
            }
        }
    }
}
