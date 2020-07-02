# AttRestNet

**AttRest(C#版本)是一个用于.net core应用的插件，它可以轻松地构建一个用于前后台分离的本地引用包，包括且不限于Vue.js/Jquery/Angular/React.~~~~**

AttRest(C#) is a pulgin for .net core application, It can build a local library for Vue.js/Jquery/Angular/React,which is the API local's reference

接入AttRest服务端，可以直接在前端的项目中，直接使用本地映射的对象，而不需要取核对URL地址

例如，在传统项目中,请求学生的姓名，方式如下：

```javascript
axios.get("http://www.xxxxx.com/student/getname",{id:1},function(d){
   console.log(d.name);
});
```

使用AttRest之后,方式变为对象调用形式，以React为例：

```javascript
React.$api.Student.getName(1,function(d){
   console.log(d.name);
});
```
