### Introduction ###




### Details ###




  * FOR DEV

你只要创建自己的Service Class即可，实现RESTService接口。

例如
org.heaer.service.file.ListService

那么当你在浏览器中访问下面的URL的时候，

http://localhost:8888/service/file/list

那么就会由org.heaer.service.file.ListService这个类进行处理。

而你不需要进行任何配置，只要注意命名匹配即可。

  * How the Framework do?

主要是参看以下几个类?

org.healer.service.RESTService

org.healer.service.RESTServiceManager

org.healer.service.RESTServlet

org.healer.service.SingletonRESTService