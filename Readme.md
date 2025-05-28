# 2025 TGC GameJam Project

## 数据配置框架说明
### CardDataBase
**配置路径：Assets/Resources/Databases/CardDatabase**   
字段说明：  
* Card Name: 卡牌名称，用于在卡牌详情界面展示
* Card Id: 卡牌Id，用于数据库检索，以及在Effect和Condition中的索引，比如insight_card
* Card Description: 卡牌描述，用于在卡牌详情界面展示
* Card Type: 卡牌类型，当前有三种：Money（资金卡），Function（功能卡），Friend（NPC卡）
* Card Image：卡牌展示的图片
* Card Effects: 卡牌功效列表，Effect配置格式见<a href="### Effect and Condition System">Effect and Condition System</a>，列表中功效依次生效


### EventDataBase
**配置路径：Assets/Resources/Databases/CardDatabase**   
字段说明：  
* Event Id: 事件Id，用于数据库检索，以及在Effect和Condition中的索引，比如insight_card_event
* Title: 事件标题，用于界面展示
* Description: 事件描述，用于界面展示
* Event Type: 事件类型，当前只有Resource一种
* is Previewable: 是否可预览， TODO：当前预览功能未实现
* Event Image：事件展示的图片
* Result: 结果列表，每个结果代表一个选项，最多3个选项
  * Description：选项描述，会展示在UI上
  * Result Image: 这个选项展示的图片
  * Event Effects: 这个选项实际产生的功效列表，Effect配置格式见<a href="### Effect and Condition System">Effect and Condition System</a>，列表中功效依次生效
* Prerequisites: 事件前置条件，可以有多个Prerequisite，只要满足其中一个便可开启事件
  * Description：当前Prerequisite的描述，暂时没用
  * Conditions：当前Prerequisite的条件集合，每个Condition之间是And的关系，必须满足所有Condition，这个Prerequisite才算被满足，Condition配置格式见<a href="### Effect and Condition System">Effect and Condition System</a>  

### ProjectDatabase
**配置路径：Assets/Resources/Databases/ProjectDatabase**   
字段说明：  
* Project Id: 项目Id，用于数据库检索，以及在Effect和Condition中的索引，比如xxx_project
* Title: 项目标题，用于界面展示
* Description: 项目描述，用于界面展示
* Project Image：项目展示的图片
* is Previewable: 是否可预览， TODO：当前预览功能未实现
* Result: 结果列表，一般配两个，一个成功，一个失败
  * Description：结果描述，用于展示
  * Effects: 结果产生的功效列表，Effect配置格式见<a href="### Effect and Condition System">Effect and Condition System</a>，列表中功效依次生效
* Must Place Cards: 必须放置的卡牌ID列表，这些卡牌必须被放置才能开始项目，比如必须放置资金卡
* Init Need Dices: 初始需要的正面骰子数，TODO：当前骰子系统未实现
* Init Card Slots: 初始卡槽数，TODO：当前卡槽系统未实现
* Prerequisites: 项目前置条件，可以有多个Prerequisite，只要满足其中一个便可开启项目
  * Description：当前Prerequisite的描述，暂时没用
  * Conditions：当前Prerequisite的条件集合，每个Condition之间是And的关系，必须满足所有Condition，这个Prerequisite才算被满足，Condition配置格式见<a href="### Effect and Condition System">Effect and Condition System</a>  



### Effect and Condition System
**用来配置卡牌的效果，或者配置事件/项目的开启条件以及结果的系统，比如我想给某个卡牌配一个骰子加2的效果，就给它配一个"Dices += 2"的Effect，或者要给某个事件配一个前置条件，可以添加一个"EventsFinished:has:insight_card_event"的Condition，代表这个事情的开启条件是之前完成过insight_card_event事件**
#### Property声明（支持操作的属性）
需要支持Effect和Condition的属性全部配置在GameManager下面，如果是数值类型（当前只支持int）就配在BaseGameProperties下面  
**BaseGameProperties字段说明：**
* Property Name：属性名称，后续在EffectCode或者ConditionCode中都使用这个名称来指代这个属性
* Current Value: 游戏运行时这个属性的当前值
* Max Value & Min Value: 属性的取值范围
* Out of Range Handle Policy: 属性超出取值范围时的处理策略，当前有两种：Clamp（夹断， 会把属性值控制在max和min之间，比如玩家手上只有一个骰子，使用了骰子-2的卡牌，如果Dices这个属性配置了Clamp且MinValue=0的话，玩家手头的骰子数量就会变为0，而不是-1），Error（报错，超出取值范围就直接报错，比如ActioPoints减到了-1，就应该直接报错，这种情况一般是游戏出现bug了）
* Explanation：属性说明，暂时没用，后面可能用在UI显示中  

**支持操作的列表当前有这几个：HandCards, EventsFinished, ProjectFinished, Friends**

#### Effect
* 当前支持的数值操作：+=，-=，*=，/=，=
* 当前支持的列表操作：add, remove, clear
* 数值操作Effect Code格式：{PropertyName} {操作符} {值}， 样例：ActionPoints -= 1（行动点-1）
* 列表操作Effect Code格式：{列表名}:{操作符}:{元素Id} HandCards:add:insight_card（往手牌中添加insight_card）， HandCards:clear （清空手牌）**注意别写成中文冒号了**

#### Condition
* 当前支持的数值比较符：>=, <=, >, <, =, !=
* 当前支持的列表判断：has(判断存在性)
* 数值Condition Code格式：{PropertyName} {比较符} {值}， 样例：ActionPoints >= 1 
* 列表Condition Code格式：{列表名}:{比较符}:{元素Id}， 样例：HandCards:has:some_npc （手牌中要有some_npc这张卡） 

## DevLog
### 05/28
[ ] 轮次结束UI & 功能  
[ ] 项目投资结算功能  
[ ] 丢骰子UI & 动画  
[ ] 项目投资骰子后台计算功能  
[ ] 项目投资界面UI绘制  
[ ] 项目投资界面UI功能  
[ ] 主界面其他属性UI绘制 & 功能  
[x] 数据配置框架说明文档  
[x] 主界面手牌显示UI绘制 & 功能  
[x] 主界面行动点UI绘制 & 功能  
[x] Effect and Condition System & 数据配置框架  
[x] Resource Event UI & 功能  
