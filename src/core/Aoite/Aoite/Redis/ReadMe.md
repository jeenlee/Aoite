##String 未实现命令

* [BITCOUNT](http://redisdoc.com/string/bitcount.html)
* [BITOP](http://redisdoc.com/string/bitop.html)
* [GETBIT](http://redisdoc.com/string/getbit.html)
* [SETBIT](http://redisdoc.com/string/setbit.html)
* [SETEX](http://redisdoc.com/string/psetex.html) 、[PSETEX](http://redisdoc.com/string/psetex.html)、[SETNX](http://redisdoc.com/string/setnx.html)：因为 [SET](http://redisdoc.com/string/set.html) 命令可以通过参数来实现这三个命令的效果。

##Key 未实现命令

* [MIGRATE](http://redisdoc.com/key/migrate.html)
* [OBJECT](http://redisdoc.com/key/object.html)
* [DUMP](http://redisdoc.com/key/dump.html)
* [RESTORE](http://redisdoc.com/key/restore.html)
* [SORT](http://redisdoc.com/key/sort.html)：为了找到更好的 SORT 设计模式，实现其他所有命令后再去实现它。

##Pub/Sub 未实现命令

* 所有命令等找到合适的方式再去实现。