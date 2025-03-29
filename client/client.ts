import * as alt from 'alt-client';
import * as native from 'natives';

const INTERACT_KEY = 0x45;
const AIM_KEY = 25;
let isInFishingZone = false;

alt.onServer("ShowNotification", (msg: string) => {
    native.beginTextCommandDisplayHelp("STRING");
    native.addTextComponentSubstringPlayerName(msg);
    native.endTextCommandDisplayHelp(0, false, true, 5000);
});

alt.onServer("SetFishingZone", (state: boolean) => {
    isInFishingZone = state;
});

alt.on('keydown', (key: number) => {
    if(isInFishingZone)
    {
        if (native.isControlPressed(0, AIM_KEY)) {
            const camPos = native.getGameplayCamCoord();
            const direction = getCameraDirection();
            const endPos = new alt.Vector3(
                camPos.x + direction.x * 20, 
                camPos.y + direction.y * 20, 
                camPos.z + direction.z * 20
            );
            
    
            let [isWater, selectedWaterPosition] = native.testProbeAgainstWater(
                camPos.x, camPos.y, camPos.z, 
                endPos.x, endPos.y, endPos.z
            );
    
            if (isWater) {
                alt.log("Aiming at Water!");
                if (key === INTERACT_KEY) {
                    alt.log("Aiming at Water! And in Fishing Spot!");
                    alt.emitServer('StartFishing', selectedWaterPosition);
                }
                else
                {
                    alt.log("You can't Fish right now!");
                }
            }
            else
            {
                alt.log("You must aim at water!");
            }
        }
    }
});

alt.onServer('PlayFishingAnimation', () => {
    const player = alt.Player.local;

    native.requestAnimDict('amb@world_human_stand_fishing@idle_a');
    alt.setTimeout(() => {
        native.taskPlayAnim(
            player,
            'amb@world_human_stand_fishing@idle_a',
            'idle_a',
            8.0,
            -8.0,
            10000,
            1,
            0,
            false,
            false,
            false
        );
    }, 500);
});

function getCameraDirection(): alt.Vector3 {
    const rot = native.getGameplayCamRot(2);
    const radX = rot.x * (Math.PI / 180);
    const radZ = rot.z * (Math.PI / 180);

    return new alt.Vector3(
        -Math.sin(radZ) * Math.cos(radX),
        Math.cos(radZ) * Math.cos(radX),
        Math.sin(radX)
    );
}