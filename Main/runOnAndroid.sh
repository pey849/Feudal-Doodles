# Instructions:
#
#   1.  Build (but don't run) your game in Unity. Save the game as "game.apk" in 
#       the same directory where this script is located.
#   2.  Connect your android device(s) to your computer.
#   3.  Run this script in any bash emulator (e.g. Git Bash)
#
#   Your game will be loaded and ran onto each connected Android device.
#
#   Important:
#       1.  In your bash emulator, make sure ~/.bashrc has this line:
#                 export PATH=$PATH:"/C/Users/<USER>/AppData/Local/Android/sdk/platform-tools"
#           This is needed for this script to locate "adb.exe"
#       2.  Ensure USB debugging mode is enabled on your connected android devices.

UNINSTALL_CLASS='com.Feudal.FeudalDoodles'
ACTIVITY_CLASS='com.Feudal.FeudalDoodles/com.unity3d.player.UnityPlayerActivity'

echo "Deploying ./*.apk to connected Android devices..."

for SERIAL in $(adb.exe devices | tail -n +2 | cut -sf 1); do 
	for APK in $(ls *.apk); do
        echo "Uninstalling any previous version of ${UNINSTALL_CLASS} on $SERIAL..."
        adb -s $SERIAL uninstall ${UNINSTALL_CLASS}
		echo "Deploying $APK on $SERIAL..."
		adb -s $SERIAL install -r $APK
        adb -s $SERIAL shell 'am start -n ' ${ACTIVITY_CLASS}
	done
done

for SERIAL in $(adb.exe devices | tail -n +2 | cut -sf 1); do 
    echo " *** Run command below in separate terminal window to view Unity and Java print statements *** "
    echo "  adb.exe -s ${SERIAL} logcat -s \"Unity\",\"PeerConnection\",\"PeerBroadcastReceiver\",\"PeerInfoListener\",\"PeerListListener\",\"ServiceDiscovery\"  "
done

echo "Script done."
