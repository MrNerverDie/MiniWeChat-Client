[![hh](http://images.cnblogs.com/cnblogs_com/neverdie/685942/o_MiniWeChat-logo.png)](https://github.com/MrNerverDie/MiniWeChat-Client)

![License](https://img.shields.io/github/license/mashape/apistatus.svg)
![Unity](https://img.shields.io/badge/Unity-4.6.3-orange.svg)
![Platform](https://img.shields.io/badge/platform-all-red.svg)
![version](https://img.shields.io/badge/version-1.0-yellow.svg)

#MiniWeChat

迷你微信服务器端：[MiniWeChat-Server](https://github.com/MrNerverDie/MiniWeChat-Server)

迷你微信是一个基于Unity3D的微信客户端，客户端使用了MVVM架构，并使用protobuf作为网络协议生成语言。

迷你微信的功能基本实现了微信的作为聊天软件的基础功能，包括用户管理，群租管理，单聊，群聊等，具体内容可以参考项目Wiki中的产品文档。

###项目需求

Unity版本 > 4.6.3

###如何部署

1. 从GitHub上直接clone该项目

	     $ git clone https://github.com/MrNerverDie/MiniWeChat-Client.git

2. 安装之后通过Unity的Open Project选项打开该项目

>  对于Windows用户，如果您想使用VisualStudio 2012打开该项目的话，您可以下载[Visual Studio 2012 Tools for Unity extension](https://visualstudiogallery.msdn.microsoft.com/7ab11d2a-f413-4ed6-b3de-ff1d05157714/)，如果您想直接下载安卓版本的apk安装文件，可以直接点击该[下载地址](http://7xiw0o.com1.z0.glb.clouddn.com/MiniWeChat.apk)

###架构

![Architecture](http://images.cnblogs.com/cnblogs_com/neverdie/685942/o_MiniWeChat%20Client%e7%9a%84MVVM.png)

###展示

####聊天界面

![Chat](http://images.cnblogs.com/cnblogs_com/neverdie/685942/o_ex01.jpg)

####消息列表

![Messages](http://images.cnblogs.com/cnblogs_com/neverdie/685942/o_ex02.jpg)

####详情管理

![Detail](http://images.cnblogs.com/cnblogs_com/neverdie/685942/o_ex03.jpg)

####用户管理

![User](http://images.cnblogs.com/cnblogs_com/neverdie/685942/o_ex04.jpg)
