# TagsTree 使用说明

## 介绍

TagsTree 是一个用树状结构标签管理文件的软件

TagsTree 能在不干涉文件本身的前提下，为软件添加标签

## 使用方法

### 1. 配置

第一次打开软件时，需要设定配置，一共有三项配置和选择主题色选项：

* 「配置路径」最好指定一个空文件夹，因为路径下会放置软件需要的文件，分别是：

1. TagsTree.xml 以树状结构存储所有标签

2. Files.json 存储所有被引入的文件

3. Relations.xml 存储文件和标签的对应关系

* 「文件路径」是你想要归类的所有文件，所在的文件夹或驱动器，以后所有引入文件都必须在这个目录下

* 「将所在路径文件夹名作为标签使用」推荐选择，选择后文件夹所在路径除了「文件路径」部分，所有文件夹名都可以作为标签搜索，但不会作为标签被软件存储

### 2. 文件引入

在**文件引入**中选择文件并引入，只有被引入的文件才能添加标签

* 选择引入：引入选择的文件或文件夹

* 目录引入：引入选择的目录下一级所有文件或文件夹或全部

* 全部引入：引入选择的目录下所有深度所有文件

* 可以选定行并按 Del 键删除

* 只允许导入「文件路径」下的文件或文件夹，不符合和重复的将被剔除

### 3. 标签管理

只有这里存在的标签才能被添加到文件上，标签管理有四个按键：

* 新建：输入「标签名称」，该标签就会作为新标签加在「标签路径」目录下

* 移动：输入「标签名称」，该标签和其目录下的所有标签都会移动到「标签路径」目录下

* 重命名：输入「标签路径」，该标签名将会改成「标签名称」

* 删除：输入「标签路径」，该标签和其目录下的所有标签都会被删除

除此之外还有一些更方便的操作：

* 在标签上或空白区域右键，可以看到一些功能：新建、剪切、粘贴、重命名、删除，除了将上面移动操作换成了剪切和粘贴，其他操作的效果与上面按键一致

* 移动标签可以直接拖动标签，标签和其目录下的所有标签都会移动到释放的标签处（如果释放在空白区域，则成为根标签）

### 4. 为标签添加文件

设置完标签和文件后，就可以建立起标签和文件的联系：

首先在树状图中选择一个标签，之后会为这个标签添加文件

选择标签后会显示所有文件，此时可以在「标签路径」处输入标签以筛选文件（详见 **5. 主页面**）

每个文件前都有一个勾选框，在刚选择完标签后，这个框会有三种状态：

* 勾选：表示这个文件已经拥有你选择的标签

* 半选：表示这个文件拥有你选择的标签的子标签（例如一个文件拥有标签**博丽灵梦**（在**东方Project**标签下），而你选择的标签是**东方Project**，则这个文件会出现半选状态）

* 未选：如果这个文件不符合上面所述两种情况，则显示未选

用户操作时，选择框只会出现两种：

* 如果原来是勾选或半选，则可以在原来的选项和未选两种之间选择

* 如果原来是未选，则可以在未选和勾选两种之间选择

最后保存时，对于每个文件，如果勾选框与原来状态相同，则不更改；如果状态不同：

* 勾选变成未选：删除文件的该标签

* 未选变成勾选：为文件添加该标签

* 半选变成未选：删除文件的该标签的所有子标签

### 5. 主页面

本页面是用来搜索和编辑文件的

在中间的文本框中输入标签的完整名称（可输入多个标签，标签间用一个空格隔开）来搜索文件，所搜索到的文件一定拥有全部指定标签或其子标签

* 如果在配置中勾选了「将所在路径文件夹名作为标签使用」，还可以将路径中的文件夹名作为标签筛选

* 如果留空，则显示引入的全部文件

一次搜索后，会出现模糊文件名搜索（英文大小写不敏感），可以输入文件名来筛选刚刚的搜索结果，模糊搜索分为三级自上而下排列

1. 完整包含搜索内容，如 gst 可以匹配到 Ta**gsT**ree

2. 有序并全部包含所有字符，如 asr 可以匹配到 T**a**g**s**T**r**ee

3. 包含任意一个字符，如 abcd 可以匹配到 T**a**gsTree

在搜索结果中，可以右键对文件进行编辑，包含打开、打开文件夹、移除、属性四个操作，而在属性中有更多操作：

* 打开、打开文件夹：打开文件或用文件资源管理器打开所在文件夹

* 编辑标签：详见 **6. 为文件添加标签**

* 移除：从软件中移除该文件（文件本体不会被删除），与引入文件相对

* 重命名、移动、删除：同文件资源管理器效果（文件不能被占用）

* 如果找不到文件，会有部分功能不能使用，可以亲自到该目录下查看是否文件名被更改或被删除，将文件名和路径与软件中改成一致就可以重新检测到了

### 6. 为文件添加标签

这个窗口只能从文件属性中打开，所以已经提前指定了文件

添加、删除可以将左侧文本框中的标签添加到该文件或从该文件下删除，按下保存后修改才会保存

---

项目名称：TagTree

项目地址：https://github.com/Poker-sang/TagsTree

版本：1.00

2021.8.22