�
UC:\sviluppo\ServiceFabricSamples\ActorModelDemo\ActorModelDemo.Core\ActorReference.cs
	namespace 	
ActorModelDemo
 
. 
Core 
{ 
[ 
DataContract 
] 
public 

class 
ActorReference 
{ 
[ 	

DataMember	 
] 
public		 
string		 

ServiceUri		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
[ 	

DataMember	 
] 
public 
string 
ActorId 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} ��
hC:\sviluppo\ServiceFabricSamples\ActorModelDemo\ActorModelDemo.Core\Collections\FineGrainQueueManager.cs
	namespace 	
ActorModelDemo
 
. 
Core 
. 
Collections )
{		 
public

 

static

 
class

 !
FineGrainQueueManager

 -
{ 
private 
static 
string $
GetQueueHeadIdentityName 6
(6 7
string7 =
	queueName> G
)G H
{ 	
return 
$" 
{ 
	queueName 
}  !
_HeadIdentity! .
". /
;/ 0
} 	
private 
static 
string  
GetQueueIdentityName 2
(2 3
string3 9
	queueName: C
)C D
{ 	
return 
$" 
{ 
	queueName 
}  !
	_Identity! *
"* +
;+ ,
} 	
private 
static 
string 
GetQueueItemKey -
(- .
string. 4
	queueName5 >
,> ?
long@ D
itemIdentityE Q
)Q R
{ 	
return 
$" 
{ 
	queueName 
}  
_  !
{! "
itemIdentity" .
}. /
"/ 0
;0 1
} 	
private 
static 
Task 
SetHeadIdentity +
(+ ,
this, 0
IActorStateManager1 C
stateD I
,I J
stringK Q
	queueNameR [
,[ \
long 
counter 
, 
CancellationToken +
cancellationToken, =
=> ?
default@ G
(G H
CancellationTokenH Y
)Y Z
)Z [
{ 	
return 
state 
. 
SetStateAsync &
<& '
long' +
>+ ,
(, -$
GetQueueHeadIdentityName- E
(E F
	queueNameF O
)O P
,P Q
counterR Y
,Y Z
cancellationToken[ l
)l m
;m n
}   	
private"" 
static"" 
async"" 
Task"" !
<""! "
long""" &
>""& '
GetHeadIdentity""( 7
(""7 8
this""8 <
IActorStateManager""= O
state""P U
,""U V
string""W ]
	queueName""^ g
,""g h
CancellationToken## 
cancellationToken## /
=##0 1
default##2 9
(##9 :
CancellationToken##: K
)##K L
)##L M
{$$ 	
var%% 
counter%% 
=%% 
await%% 
state%%  %
.%%% &
TryGetStateAsync%%& 6
<%%6 7
long%%7 ;
>%%; <
(%%< =$
GetQueueHeadIdentityName%%= U
(%%U V
	queueName%%V _
)%%_ `
,%%` a
cancellationToken%%b s
)%%s t
;%%t u
return&& 
counter&& 
.&& 
HasValue&& #
?&&$ %
counter&&& -
.&&- .
Value&&. 3
:&&4 5
$num&&6 7
;&&7 8
}'' 	
private)) 
static)) 
async)) 
Task)) !
<))! "
long))" &
>))& '
GetIdentity))( 3
())3 4
this))4 8
IActorStateManager))9 K
state))L Q
,))Q R
string))S Y
	queueName))Z c
,))c d
CancellationToken** 
cancellationToken** /
=**0 1
default**2 9
(**9 :
CancellationToken**: K
)**K L
)**L M
{++ 	
var,, 
identity,, 
=,, 
await,,  
state,,! &
.,,& '
TryGetStateAsync,,' 7
<,,7 8
long,,8 <
>,,< =
(,,= > 
GetQueueIdentityName,,> R
(,,R S
	queueName,,S \
),,\ ]
,,,] ^
cancellationToken,,_ p
),,p q
;,,q r
return-- 
identity-- 
.-- 
HasValue-- $
?--% &
identity--' /
.--/ 0
Value--0 5
:--6 7
---8 9
$num--9 :
;--: ;
}.. 	
private00 
static00 
Task00 
SetIdentity00 '
(00' (
this00( ,
IActorStateManager00- ?
state00@ E
,00E F
string00G M
	queueName00N W
,00W X
long11 
identity11 
,11 
CancellationToken11 ,
cancellationToken11- >
=11? @
default11A H
(11H I
CancellationToken11I Z
)11Z [
)11[ \
{22 	
return33 
state33 
.33 
SetStateAsync33 &
<33& '
long33' +
>33+ ,
(33, - 
GetQueueIdentityName33- A
(33A B
	queueName33B K
)33K L
,33L M
identity33N V
,33V W
cancellationToken33X i
)33i j
;33j k
}44 	
public99 
static99 
async99 
Task99  
EnqueueAsync99! -
<99- .
TElement99. 6
>996 7
(997 8
this998 <
IActorStateManager99= O
state99P U
,99U V
string99W ]
	queueName99^ g
,99g h
TElement:: 
element:: 
,:: 
CancellationToken:: /
cancellationToken::0 A
=::B C
default::D K
(::K L
CancellationToken::L ]
)::] ^
)::^ _
{;; 	
if<< 
(<< 
state<< 
==<< 
null<< 
)<< 
throw== 
new== "
NullReferenceException== 0
(==0 1
nameof==1 7
(==7 8
state==8 =
)=== >
)==> ?
;==? @
if>> 
(>> 
string>> 
.>> 
IsNullOrWhiteSpace>> )
(>>) *
	queueName>>* 3
)>>3 4
)>>4 5
throw?? 
new?? 
ArgumentException?? +
(??+ ,
nameof??, 2
(??2 3
	queueName??3 <
)??< =
)??= >
;??> ?
varAA 
currentIdentityAA 
=AA  !
awaitAA" '
stateAA( -
.AA- .
GetIdentityAA. 9
(AA9 :
	queueNameAA: C
,AAC D
cancellationTokenAAE V
)AAV W
;AAW X
ifBB 
(BB 
!BB 
cancellationTokenBB "
.BB" ##
IsCancellationRequestedBB# :
)BB: ;
{CC 
currentIdentityDD 
=DD  !
awaitDD" '
stateDD( -
.DD- .
AddItemToStateAsyncDD. A
(DDA B
	queueNameDDB K
,DDK L
elementDDM T
,DDT U
currentIdentityDDV e
,DDe f
cancellationTokenDDg x
)DDx y
;DDy z
awaitEE 
stateEE 
.EE 
SetIdentityEE '
(EE' (
	queueNameEE( 1
,EE1 2
currentIdentityEE3 B
,EEB C
cancellationTokenEED U
)EEU V
;EEV W
}FF 
}GG 	
privateTT 
staticTT 
asyncTT 
TaskTT !
<TT! "
longTT" &
>TT& '
AddItemToStateAsyncTT( ;
<TT; <
TElementTT< D
>TTD E
(TTE F
thisTTF J
IActorStateManagerTTK ]
stateTT^ c
,TTc d
stringTTe k
	queueNameTTl u
,TTu v
TElementUU 
elementUU 
,UU 
longUU "
currentIdentityUU# 2
,UU2 3
CancellationTokenUU4 E
cancellationTokenUUF W
=UUX Y
defaultUUZ a
(UUa b
CancellationTokenUUb s
)UUs t
)UUt u
{VV 	
ifWW 
(WW 
currentIdentityWW 
==WW  "
longWW# '
.WW' (
MaxValueWW( 0
)WW0 1
{XX 
varYY 
currentHeadYY 
=YY  !
awaitYY" '
stateYY( -
.YY- .
GetHeadIdentityYY. =
(YY= >
	queueNameYY> G
,YYG H
cancellationTokenYYI Z
)YYZ [
;YY[ \
ifZZ 
(ZZ 
currentHeadZZ 
==ZZ  "
$numZZ# $
)ZZ$ %
throw[[ 
new[[ '
InsufficientMemoryException[[ 9
([[9 :
$str[[: [
)[[[ \
;[[\ ]
currentIdentity\\ 
=\\  !
$num\\" #
;\\# $
}]] 
else^^ 
{__ 
currentIdentity`` 
++`` !
;``! "
}aa 
varbb 
itemKeybb 
=bb 
GetQueueItemKeybb )
(bb) *
	queueNamebb* 3
,bb3 4
currentIdentitybb5 D
)bbD E
;bbE F
awaitcc 
statecc 
.cc !
AddOrUpdateStateAsynccc -
(cc- .
itemKeycc. 5
,cc5 6
elementcc7 >
,cc> ?
(cc@ A
kccA B
,ccB C
vccD E
)ccE F
=>ccG I
elementccJ Q
,ccQ R
cancellationTokenccS d
)ccd e
;cce f
returndd 
currentIdentitydd "
;dd" #
}ee 	
publictt 
statictt 
asynctt 
Tasktt  
EnqueueAsynctt! -
<tt- .
TElementtt. 6
>tt6 7
(tt7 8
thistt8 <
IActorStateManagertt= O
statettP U
,ttU V
stringttW ]
	queueNamett^ g
,ttg h
IEnumerableuu 
<uu 
TElementuu  
>uu  !
elementsuu" *
,uu* +
CancellationTokenuu, =
cancellationTokenuu> O
=uuP Q
defaultuuR Y
(uuY Z
CancellationTokenuuZ k
)uuk l
)uul m
{vv 	
ifww 
(ww 
stateww 
==ww 
nullww 
)ww 
throwxx 
newxx "
NullReferenceExceptionxx 0
(xx0 1
nameofxx1 7
(xx7 8
statexx8 =
)xx= >
)xx> ?
;xx? @
ifyy 
(yy 
stringyy 
.yy 
IsNullOrWhiteSpaceyy )
(yy) *
	queueNameyy* 3
)yy3 4
)yy4 5
throwzz 
newzz 
ArgumentExceptionzz +
(zz+ ,
nameofzz, 2
(zz2 3
	queueNamezz3 <
)zz< =
)zz= >
;zz> ?
if{{ 
({{ 
elements{{ 
=={{ 
null{{  
){{  !
throw|| 
new|| !
ArgumentNullException|| /
(||/ 0
nameof||0 6
(||6 7
elements||7 ?
)||? @
)||@ A
;||A B
if~~ 
(~~ 
elements~~ 
.~~ 
Any~~ 
(~~ 
)~~ 
)~~ 
{ 
var
�� 
currentIdentity
�� #
=
��$ %
await
��& +
state
��, 1
.
��1 2
GetIdentity
��2 =
(
��= >
	queueName
��> G
,
��G H
cancellationToken
��I Z
)
��Z [
;
��[ \
for
�� 
(
�� 
int
�� 
	itemIndex
�� "
=
��# $
$num
��% &
;
��& '
	itemIndex
��( 1
<
��2 3
elements
��4 <
.
��< =
Count
��= B
(
��B C
)
��C D
;
��D E
	itemIndex
��F O
++
��O Q
)
��Q R
{
�� 
cancellationToken
�� %
.
��% &*
ThrowIfCancellationRequested
��& B
(
��B C
)
��C D
;
��D E
var
�� 
element
�� 
=
��  !
elements
��" *
.
��* +
	ElementAt
��+ 4
(
��4 5
	itemIndex
��5 >
)
��> ?
;
��? @
currentIdentity
�� #
=
��$ %
await
��& +
state
��, 1
.
��1 2!
AddItemToStateAsync
��2 E
(
��E F
	queueName
��F O
,
��O P
element
��Q X
,
��X Y
currentIdentity
��Z i
,
��i j
cancellationToken
��k |
)
��| }
;
��} ~
}
�� 
await
�� 
state
�� 
.
�� 
SetIdentity
�� '
(
��' (
	queueName
��( 1
,
��1 2
currentIdentity
��3 B
,
��B C
cancellationToken
��D U
)
��U V
;
��V W
}
�� 
}
�� 	
public
�� 
static
�� 
async
�� 
Task
��  
<
��  !
TElement
��! )
>
��) *
DequeueAsync
��+ 7
<
��7 8
TElement
��8 @
>
��@ A
(
��A B
this
��B F 
IActorStateManager
��G Y
state
��Z _
,
��_ `
string
��a g
	queueName
��h q
,
��q r
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
(
��9 :
CancellationToken
��: K
)
��K L
)
��L M
{
�� 	
if
�� 
(
�� 
state
�� 
==
�� 
null
�� 
)
�� 
throw
�� 
new
�� $
NullReferenceException
�� 0
(
��0 1
nameof
��1 7
(
��7 8
state
��8 =
)
��= >
)
��> ?
;
��? @
if
�� 
(
�� 
string
�� 
.
��  
IsNullOrWhiteSpace
�� )
(
��) *
	queueName
��* 3
)
��3 4
)
��4 5
throw
�� 
new
�� 
ArgumentException
�� +
(
��+ ,
nameof
��, 2
(
��2 3
	queueName
��3 <
)
��< =
)
��= >
;
��> ?
TElement
�� 
result
�� 
=
�� 
default
�� %
(
��% &
TElement
��& .
)
��. /
;
��/ 0
if
�� 
(
�� 
await
�� 
state
�� 
.
�� !
GetQueueLengthAsync
�� /
(
��/ 0
	queueName
��0 9
,
��9 :
cancellationToken
��; L
)
��L M
>
��N O
$num
��P Q
)
��Q R
{
�� 
var
�� !
currentHeadIdentity
�� '
=
��( )
await
��* /
state
��0 5
.
��5 6
GetHeadIdentity
��6 E
(
��E F
	queueName
��F O
,
��O P
cancellationToken
��Q b
)
��b c
;
��c d
var
�� 
currentIdentity
�� #
=
��$ %
await
��& +
state
��, 1
.
��1 2
GetIdentity
��2 =
(
��= >
	queueName
��> G
,
��G H
cancellationToken
��I Z
)
��Z [
;
��[ \
var
�� 
headGTIdentity
�� "
=
��# $!
currentHeadIdentity
��% 8
>
��9 :
currentIdentity
��; J
;
��J K
var
�� 
itemKey
�� 
=
�� 
GetQueueItemKey
�� -
(
��- .
	queueName
��. 7
,
��7 8!
currentHeadIdentity
��9 L
)
��L M
;
��M N
var
�� 
element
�� 
=
�� 
await
�� #
state
��$ )
.
��) *
TryGetStateAsync
��* :
<
��: ;
TElement
��; C
>
��C D
(
��D E
itemKey
��E L
,
��L M
cancellationToken
��N _
)
��_ `
;
��` a
if
�� 
(
�� 
element
�� 
.
�� 
HasValue
�� $
)
��$ %
{
�� 
await
�� 
state
�� 
.
��  !
TryRemoveStateAsync
��  3
(
��3 4
itemKey
��4 ;
,
��; <
cancellationToken
��= N
)
��N O
;
��O P
result
�� 
=
�� 
element
�� $
.
��$ %
Value
��% *
;
��* +
if
�� 
(
�� !
currentHeadIdentity
�� +
==
��, .
long
��/ 3
.
��3 4
MaxValue
��4 <
)
��< =
{
�� !
currentHeadIdentity
�� +
=
��, -
$num
��. /
;
��/ 0
}
�� 
else
�� 
{
�� !
currentHeadIdentity
�� +
++
��+ -
;
��- .
}
�� 
if
�� 
(
�� !
currentHeadIdentity
�� +
>
��, -
currentIdentity
��. =
&&
��> @
!
��A B
headGTIdentity
��B P
)
��P Q
{
�� 
await
�� 
state
�� #
.
��# $
SetIdentity
��$ /
(
��/ 0
	queueName
��0 9
,
��9 :
-
��; <
$num
��< =
,
��= >
cancellationToken
��? P
)
��P Q
;
��Q R
await
�� 
state
�� #
.
��# $
SetHeadIdentity
��$ 3
(
��3 4
	queueName
��4 =
,
��= >
$num
��? @
,
��@ A
cancellationToken
��B S
)
��S T
;
��T U
}
�� 
else
�� 
{
�� 
await
�� 
state
�� #
.
��# $
SetHeadIdentity
��$ 3
(
��3 4
	queueName
��4 =
,
��= >!
currentHeadIdentity
��? R
,
��R S
cancellationToken
��T e
)
��e f
;
��f g
}
�� 
}
�� 
}
�� 
return
�� 
result
�� 
;
�� 
}
�� 	
public
�� 
static
�� 
async
�� 
Task
��  
<
��  !
bool
��! %
>
��% &

PurgeQueue
��' 1
(
��1 2
this
��2 6 
IActorStateManager
��7 I
state
��J O
,
��O P
string
��Q W
	queueName
��X a
,
��a b
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
(
��9 :
CancellationToken
��: K
)
��K L
)
��L M
{
�� 	
if
�� 
(
�� 
state
�� 
==
�� 
null
�� 
)
�� 
throw
�� 
new
�� $
NullReferenceException
�� 0
(
��0 1
nameof
��1 7
(
��7 8
state
��8 =
)
��= >
)
��> ?
;
��? @
if
�� 
(
�� 
string
�� 
.
��  
IsNullOrWhiteSpace
�� )
(
��) *
	queueName
��* 3
)
��3 4
)
��4 5
throw
�� 
new
�� 
ArgumentException
�� +
(
��+ ,
nameof
��, 2
(
��2 3
	queueName
��3 <
)
��< =
)
��= >
;
��> ?
var
�� 
queueItemKeys
�� 
=
�� 
(
��  !
await
��! &
state
��' ,
.
��, - 
GetStateNamesAsync
��- ?
(
��? @
cancellationToken
��@ Q
)
��Q R
)
��R S
.
�� 
Where
�� 
(
�� 
k
�� 
=>
�� 
k
�� 
.
�� 

StartsWith
�� (
(
��( )
$"
��) +
{
��+ ,
	queueName
��, 5
}
��5 6
_
��6 7
"
��7 8
)
��8 9
)
��9 :
.
��: ;
ToList
��; A
(
��A B
)
��B C
;
��C D
bool
�� 
result
�� 
=
�� 
true
�� 
;
�� 
foreach
�� 
(
�� 
var
�� 
queueItemKey
�� %
in
��& (
queueItemKeys
��) 6
)
��6 7
{
�� 
result
�� 
=
�� 
result
�� 
&
��  !
await
��" '
state
��( -
.
��- .!
TryRemoveStateAsync
��. A
(
��A B
queueItemKey
��B N
,
��N O
cancellationToken
��P a
)
��a b
;
��b c
}
�� 
await
�� 
state
�� 
.
�� 
SetIdentity
�� #
(
��# $
	queueName
��$ -
,
��- .
-
��/ 0
$num
��0 1
,
��1 2
cancellationToken
��3 D
)
��D E
;
��E F
await
�� 
state
�� 
.
�� 
SetHeadIdentity
�� '
(
��' (
	queueName
��( 1
,
��1 2
$num
��3 4
,
��4 5
cancellationToken
��6 G
)
��G H
;
��H I
return
�� 
result
�� 
;
�� 
}
�� 	
public
�� 
static
�� 
async
�� 
Task
��  
<
��  !
long
��! %
>
��% &!
GetQueueLengthAsync
��' :
(
��: ;
this
��; ? 
IActorStateManager
��@ R
state
��S X
,
��X Y
string
��Z `
	queueName
��a j
,
��j k
CancellationToken
�� $
cancellationToken
��% 6
=
��7 8
default
��9 @
(
��@ A
CancellationToken
��A R
)
��R S
)
��S T
{
�� 	
if
�� 
(
�� 
state
�� 
==
�� 
null
�� 
)
�� 
throw
�� 
new
�� $
NullReferenceException
�� 0
(
��0 1
nameof
��1 7
(
��7 8
state
��8 =
)
��= >
)
��> ?
;
��? @
if
�� 
(
�� 
string
�� 
.
��  
IsNullOrWhiteSpace
�� )
(
��) *
	queueName
��* 3
)
��3 4
)
��4 5
throw
�� 
new
�� 
ArgumentException
�� +
(
��+ ,
nameof
��, 2
(
��2 3
	queueName
��3 <
)
��< =
)
��= >
;
��> ?
long
�� 
counter
�� 
=
�� 
$num
�� 
;
�� 
var
�� 
currentIdentity
�� 
=
��  !
await
��" '
state
��( -
.
��- .
GetIdentity
��. 9
(
��9 :
	queueName
��: C
,
��C D
cancellationToken
��E V
)
��V W
;
��W X
var
�� 
currentHead
�� 
=
�� 
await
�� #
state
��$ )
.
��) *
GetHeadIdentity
��* 9
(
��9 :
	queueName
��: C
,
��C D
cancellationToken
��E V
)
��V W
;
��W X
if
�� 
(
�� 
(
�� 
currentIdentity
��  
==
��! #
-
��$ %
$num
��% &
&&
��' )
currentHead
��* 5
==
��6 8
$num
��9 :
)
��: ;
||
��< >
currentHead
��? J
<=
��K M
currentIdentity
��N ]
)
��] ^
{
�� 
counter
�� 
+=
�� 
(
�� 
currentIdentity
�� +
-
��, -
currentHead
��. 9
+
��: ;
$num
��< =
)
��= >
;
��> ?
}
�� 
else
�� 
{
�� 
counter
�� 
+=
�� 
(
�� 
(
�� 
long
�� !
.
��! "
MaxValue
��" *
-
��+ ,
currentHead
��- 8
+
��9 :
$num
��; <
)
��< =
+
��> ?
(
��@ A
currentIdentity
��A P
+
��Q R
$num
��S T
)
��T U
)
��U V
;
��V W
}
�� 
return
�� 
counter
�� 
;
�� 
}
�� 	
public
�� 
static
�� 
async
�� 
Task
��  
<
��  !
TElement
��! )
>
��) *
PeekQueueAsync
��+ 9
<
��9 :
TElement
��: B
>
��B C
(
��C D
this
��D H 
IActorStateManager
��I [
state
��\ a
,
��a b
string
��c i
	queueName
��j s
,
��s t
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
(
��9 :
CancellationToken
��: K
)
��K L
)
��L M
{
�� 	
if
�� 
(
�� 
state
�� 
==
�� 
null
�� 
)
�� 
throw
�� 
new
�� $
NullReferenceException
�� 0
(
��0 1
nameof
��1 7
(
��7 8
state
��8 =
)
��= >
)
��> ?
;
��? @
if
�� 
(
�� 
string
�� 
.
��  
IsNullOrWhiteSpace
�� )
(
��) *
	queueName
��* 3
)
��3 4
)
��4 5
throw
�� 
new
�� 
ArgumentException
�� +
(
��+ ,
nameof
��, 2
(
��2 3
	queueName
��3 <
)
��< =
)
��= >
;
��> ?
TElement
�� 
result
�� 
=
�� 
default
�� %
(
��% &
TElement
��& .
)
��. /
;
��/ 0
if
�� 
(
�� 
await
�� 
state
�� 
.
�� !
GetQueueLengthAsync
�� /
(
��/ 0
	queueName
��0 9
,
��9 :
cancellationToken
��; L
)
��L M
>
��N O
$num
��P Q
)
��Q R
{
�� 
var
�� !
currentHeadIdentity
�� '
=
��( )
await
��* /
state
��0 5
.
��5 6
GetHeadIdentity
��6 E
(
��E F
	queueName
��F O
,
��O P
cancellationToken
��Q b
)
��b c
;
��c d
var
�� 
itemKey
�� 
=
�� 
GetQueueItemKey
�� -
(
��- .
	queueName
��. 7
,
��7 8!
currentHeadIdentity
��9 L
)
��L M
;
��M N
var
�� 
element
�� 
=
�� 
await
�� #
state
��$ )
.
��) *
TryGetStateAsync
��* :
<
��: ;
TElement
��; C
>
��C D
(
��D E
itemKey
��E L
,
��L M
cancellationToken
��N _
)
��_ `
;
��` a
if
�� 
(
�� 
element
�� 
.
�� 
HasValue
�� $
)
��$ %
{
�� 
result
�� 
=
�� 
element
�� $
.
��$ %
Value
��% *
;
��* +
}
�� 
}
�� 
return
�� 
result
�� 
;
�� 
}
�� 	
}
�� 
}�� �

jC:\sviluppo\ServiceFabricSamples\ActorModelDemo\ActorModelDemo.Core\Extensions\ActorReferenceExtensions.cs
	namespace 	
ActorModelDemo
 
. 
Core 
. 

Extensions (
{		 
public

 

static

 
class

 $
ActorReferenceExtensions

 0
{ 
public 
static 
ActorReference $
ToActorReference% 5
(5 6
this6 :
Actor; @
actorA F
)F G
{ 	
if 
( 
actor 
== 
null 
) 
throw 
new "
NullReferenceException 0
(0 1
nameof1 7
(7 8
actor8 =
)= >
)> ?
;? @
return 
new 
ActorReference %
(% &
)& '
{ 
ActorId 
= 
actor 
.  
Id  "
." #
GetStringId# .
(. /
)/ 0
,0 1

ServiceUri 
= 
actor "
." #

ServiceUri# -
.- .
AbsoluteUri. 9
} 
; 
} 	
} 
} �
^C:\sviluppo\ServiceFabricSamples\ActorModelDemo\ActorModelDemo.Core\Properties\AssemblyInfo.cs
[ 
assembly 	
:	 

AssemblyTitle 
( 
$str .
). /
]/ 0
[		 
assembly		 	
:			 

AssemblyDescription		 
(		 
$str		 !
)		! "
]		" #
[

 
assembly

 	
:

	 
!
AssemblyConfiguration

  
(

  !
$str

! #
)

# $
]

$ %
[ 
assembly 	
:	 

AssemblyCompany 
( 
$str 
) 
] 
[ 
assembly 	
:	 

AssemblyProduct 
( 
$str 0
)0 1
]1 2
[ 
assembly 	
:	 

AssemblyCopyright 
( 
$str 0
)0 1
]1 2
[ 
assembly 	
:	 

AssemblyTrademark 
( 
$str 
)  
]  !
[ 
assembly 	
:	 

AssemblyCulture 
( 
$str 
) 
] 
[ 
assembly 	
:	 


ComVisible 
( 
false 
) 
] 
[ 
assembly 	
:	 

Guid 
( 
$str 6
)6 7
]7 8
[## 
assembly## 	
:##	 

AssemblyVersion## 
(## 
$str## $
)##$ %
]##% &
[$$ 
assembly$$ 	
:$$	 

AssemblyFileVersion$$ 
($$ 
$str$$ (
)$$( )
]$$) *