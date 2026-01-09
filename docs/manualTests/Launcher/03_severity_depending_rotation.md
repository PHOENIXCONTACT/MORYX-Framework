# Test Case 3 - Severity depending rotation

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
|Create two notifactions with the severity **Info**|the latest info notification should be visible in the notification bar||
|Create two notifactions with the severity **Warning**|The latest Warning notification should be visible in the notification bar||
|Create two notifactions with the severity **Error**|The latest Error notification should be visible in the notification bar||
|Create two notifactions with the severity **Fatal**|The latest Fatal notification should be visible in the notification bar||
|Acknoledge one **Fatal** notification|The notification bar only displays the latest **Fatal** notification ||
|Acknoledge one **Fatal** notification|Only the latest notification with the severity **Error** is visible in the notification bar ||
|Acknoledge one **Error** notification|The notification bar only displays the latest **Error** notification ||
|Acknoledge one **Error** notification|Only the latest notifications with the severity **Warning** is visible in the notification bar ||
|Acknoledge one **Warning** notification|The notification bar only displays the latest **Warning** notification ||
|Acknoledge one **Warning** notification|Only the latest notification with the severity **Info** is visible in the notification bar ||
|Acknoledge one **Info** notification|The notification bar only displays the remaining **Info** notification ||
|Acknoledge one **Info** notification|No notifications are displayed in the notification bar ||
