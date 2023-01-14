# GL.Cache.ExpireTime
生成固定时间（缓存过期时间）（可用于多机器部署时的缓存不一致问题）
1. 内部没有没有任何特殊写法或者类库，所以可支持多版本，自行下载或拷贝代码使用都行
2. 支持指定 小时/分钟/秒 三种类型的任意过期间隔，支持返回两种类型的时间，一种是距今多少间隔，一种是具体的年月日时分秒

### Test输出效果如下，具体调用方式见单元测试：

开始输出最近10次的时间，当前时间为：2023/1/14 14:10:02 

1. 2023/1/14 14:20:00，过期分钟为：9.957316675，组合当前时间后的新时间为：2023/1/14 14:20:00
2. 2023/1/14 14:30:00，过期分钟为：19.957319061666666，组合当前时间后的新时间为：2023/1/14 14:30:00
3. 2023/1/14 14:40:00，过期分钟为：29.95731904166667，组合当前时间后的新时间为：2023/1/14 14:40:00
4. 2023/1/14 14:50:00，过期分钟为：39.957319025，组合当前时间后的新时间为：2023/1/14 14:50:00
5. 2023/1/14 15:00:00，过期分钟为：49.957318996666665，组合当前时间后的新时间为：2023/1/14 15:00:00
6. 2023/1/14 15:10:00，过期分钟为：59.95731898，组合当前时间后的新时间为：2023/1/14 15:10:00
7. 2023/1/14 15:20:00，过期分钟为：69.95731896333334，组合当前时间后的新时间为：2023/1/14 15:20:00
8. 2023/1/14 15:30:00，过期分钟为：79.95731889，组合当前时间后的新时间为：2023/1/14 15:30:00
9. 2023/1/14 15:40:00，过期分钟为：89.95731887333334，组合当前时间后的新时间为：2023/1/14 15:40:00
10. 2023/1/14 15:50:00，过期分钟为：99.95731885666666，组合当前时间后的新时间为：2023/1/14 15:50:00


### 设定间隔10分钟过期（返回DateTime对象，适用于在指定具体时间的场景）
```C#
DateTime nextExpireTime = ExpireTimeUtil.GetNextExpireTime(10, IntervalType.Minute);
```

### 设定间隔10分钟过期（返回double类型的分钟值，适用于在指定多少分钟后的场景）
```C#
double nextExpireMinute = ExpireTimeUtil.GetNextExpireMinute(10, IntervalType.Minute);
```
