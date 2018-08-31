Å
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
} ∑’
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
ÄÄ 
currentIdentity
ÄÄ #
=
ÄÄ$ %
await
ÄÄ& +
state
ÄÄ, 1
.
ÄÄ1 2
GetIdentity
ÄÄ2 =
(
ÄÄ= >
	queueName
ÄÄ> G
,
ÄÄG H
cancellationToken
ÄÄI Z
)
ÄÄZ [
;
ÄÄ[ \
for
ÇÇ 
(
ÇÇ 
int
ÇÇ 
	itemIndex
ÇÇ "
=
ÇÇ# $
$num
ÇÇ% &
;
ÇÇ& '
	itemIndex
ÇÇ( 1
<
ÇÇ2 3
elements
ÇÇ4 <
.
ÇÇ< =
Count
ÇÇ= B
(
ÇÇB C
)
ÇÇC D
;
ÇÇD E
	itemIndex
ÇÇF O
++
ÇÇO Q
)
ÇÇQ R
{
ÉÉ 
cancellationToken
ÑÑ %
.
ÑÑ% &*
ThrowIfCancellationRequested
ÑÑ& B
(
ÑÑB C
)
ÑÑC D
;
ÑÑD E
var
ÖÖ 
element
ÖÖ 
=
ÖÖ  !
elements
ÖÖ" *
.
ÖÖ* +
	ElementAt
ÖÖ+ 4
(
ÖÖ4 5
	itemIndex
ÖÖ5 >
)
ÖÖ> ?
;
ÖÖ? @
currentIdentity
ÜÜ #
=
ÜÜ$ %
await
ÜÜ& +
state
ÜÜ, 1
.
ÜÜ1 2!
AddItemToStateAsync
ÜÜ2 E
(
ÜÜE F
	queueName
ÜÜF O
,
ÜÜO P
element
ÜÜQ X
,
ÜÜX Y
currentIdentity
ÜÜZ i
,
ÜÜi j
cancellationToken
ÜÜk |
)
ÜÜ| }
;
ÜÜ} ~
}
áá 
await
àà 
state
àà 
.
àà 
SetIdentity
àà '
(
àà' (
	queueName
àà( 1
,
àà1 2
currentIdentity
àà3 B
,
ààB C
cancellationToken
ààD U
)
ààU V
;
ààV W
}
ââ 
}
ää 	
public
èè 
static
èè 
async
èè 
Task
èè  
<
èè  !
TElement
èè! )
>
èè) *
DequeueAsync
èè+ 7
<
èè7 8
TElement
èè8 @
>
èè@ A
(
èèA B
this
èèB F 
IActorStateManager
èèG Y
state
èèZ _
,
èè_ `
string
èèa g
	queueName
èèh q
,
èèq r
CancellationToken
êê 
cancellationToken
êê /
=
êê0 1
default
êê2 9
(
êê9 :
CancellationToken
êê: K
)
êêK L
)
êêL M
{
ëë 	
if
íí 
(
íí 
state
íí 
==
íí 
null
íí 
)
íí 
throw
ìì 
new
ìì $
NullReferenceException
ìì 0
(
ìì0 1
nameof
ìì1 7
(
ìì7 8
state
ìì8 =
)
ìì= >
)
ìì> ?
;
ìì? @
if
îî 
(
îî 
string
îî 
.
îî  
IsNullOrWhiteSpace
îî )
(
îî) *
	queueName
îî* 3
)
îî3 4
)
îî4 5
throw
ïï 
new
ïï 
ArgumentException
ïï +
(
ïï+ ,
nameof
ïï, 2
(
ïï2 3
	queueName
ïï3 <
)
ïï< =
)
ïï= >
;
ïï> ?
TElement
óó 
result
óó 
=
óó 
default
óó %
(
óó% &
TElement
óó& .
)
óó. /
;
óó/ 0
if
ôô 
(
ôô 
await
ôô 
state
ôô 
.
ôô !
GetQueueLengthAsync
ôô /
(
ôô/ 0
	queueName
ôô0 9
,
ôô9 :
cancellationToken
ôô; L
)
ôôL M
>
ôôN O
$num
ôôP Q
)
ôôQ R
{
öö 
var
õõ !
currentHeadIdentity
õõ '
=
õõ( )
await
õõ* /
state
õõ0 5
.
õõ5 6
GetHeadIdentity
õõ6 E
(
õõE F
	queueName
õõF O
,
õõO P
cancellationToken
õõQ b
)
õõb c
;
õõc d
var
úú 
currentIdentity
úú #
=
úú$ %
await
úú& +
state
úú, 1
.
úú1 2
GetIdentity
úú2 =
(
úú= >
	queueName
úú> G
,
úúG H
cancellationToken
úúI Z
)
úúZ [
;
úú[ \
var
ùù 
headGTIdentity
ùù "
=
ùù# $!
currentHeadIdentity
ùù% 8
>
ùù9 :
currentIdentity
ùù; J
;
ùùJ K
var
üü 
itemKey
üü 
=
üü 
GetQueueItemKey
üü -
(
üü- .
	queueName
üü. 7
,
üü7 8!
currentHeadIdentity
üü9 L
)
üüL M
;
üüM N
var
†† 
element
†† 
=
†† 
await
†† #
state
††$ )
.
††) *
TryGetStateAsync
††* :
<
††: ;
TElement
††; C
>
††C D
(
††D E
itemKey
††E L
,
††L M
cancellationToken
††N _
)
††_ `
;
††` a
if
¢¢ 
(
¢¢ 
element
¢¢ 
.
¢¢ 
HasValue
¢¢ $
)
¢¢$ %
{
££ 
await
•• 
state
•• 
.
••  !
TryRemoveStateAsync
••  3
(
••3 4
itemKey
••4 ;
,
••; <
cancellationToken
••= N
)
••N O
;
••O P
result
¶¶ 
=
¶¶ 
element
¶¶ $
.
¶¶$ %
Value
¶¶% *
;
¶¶* +
if
©© 
(
©© !
currentHeadIdentity
©© +
==
©©, .
long
©©/ 3
.
©©3 4
MaxValue
©©4 <
)
©©< =
{
™™ !
currentHeadIdentity
´´ +
=
´´, -
$num
´´. /
;
´´/ 0
}
¨¨ 
else
≠≠ 
{
ÆÆ !
currentHeadIdentity
ØØ +
++
ØØ+ -
;
ØØ- .
}
∞∞ 
if
≥≥ 
(
≥≥ !
currentHeadIdentity
≥≥ +
>
≥≥, -
currentIdentity
≥≥. =
&&
≥≥> @
!
≥≥A B
headGTIdentity
≥≥B P
)
≥≥P Q
{
¥¥ 
await
∂∂ 
state
∂∂ #
.
∂∂# $
SetIdentity
∂∂$ /
(
∂∂/ 0
	queueName
∂∂0 9
,
∂∂9 :
-
∂∂; <
$num
∂∂< =
,
∂∂= >
cancellationToken
∂∂? P
)
∂∂P Q
;
∂∂Q R
await
∑∑ 
state
∑∑ #
.
∑∑# $
SetHeadIdentity
∑∑$ 3
(
∑∑3 4
	queueName
∑∑4 =
,
∑∑= >
$num
∑∑? @
,
∑∑@ A
cancellationToken
∑∑B S
)
∑∑S T
;
∑∑T U
}
∏∏ 
else
ππ 
{
∫∫ 
await
ªª 
state
ªª #
.
ªª# $
SetHeadIdentity
ªª$ 3
(
ªª3 4
	queueName
ªª4 =
,
ªª= >!
currentHeadIdentity
ªª? R
,
ªªR S
cancellationToken
ªªT e
)
ªªe f
;
ªªf g
}
ºº 
}
ΩΩ 
}
ææ 
return
øø 
result
øø 
;
øø 
}
¿¿ 	
public
≈≈ 
static
≈≈ 
async
≈≈ 
Task
≈≈  
<
≈≈  !
bool
≈≈! %
>
≈≈% &

PurgeQueue
≈≈' 1
(
≈≈1 2
this
≈≈2 6 
IActorStateManager
≈≈7 I
state
≈≈J O
,
≈≈O P
string
≈≈Q W
	queueName
≈≈X a
,
≈≈a b
CancellationToken
∆∆ 
cancellationToken
∆∆ /
=
∆∆0 1
default
∆∆2 9
(
∆∆9 :
CancellationToken
∆∆: K
)
∆∆K L
)
∆∆L M
{
«« 	
if
»» 
(
»» 
state
»» 
==
»» 
null
»» 
)
»» 
throw
…… 
new
…… $
NullReferenceException
…… 0
(
……0 1
nameof
……1 7
(
……7 8
state
……8 =
)
……= >
)
……> ?
;
……? @
if
   
(
   
string
   
.
    
IsNullOrWhiteSpace
   )
(
  ) *
	queueName
  * 3
)
  3 4
)
  4 5
throw
ÀÀ 
new
ÀÀ 
ArgumentException
ÀÀ +
(
ÀÀ+ ,
nameof
ÀÀ, 2
(
ÀÀ2 3
	queueName
ÀÀ3 <
)
ÀÀ< =
)
ÀÀ= >
;
ÀÀ> ?
var
ÕÕ 
queueItemKeys
ÕÕ 
=
ÕÕ 
(
ÕÕ  !
await
ÕÕ! &
state
ÕÕ' ,
.
ÕÕ, - 
GetStateNamesAsync
ÕÕ- ?
(
ÕÕ? @
cancellationToken
ÕÕ@ Q
)
ÕÕQ R
)
ÕÕR S
.
ŒŒ 
Where
ŒŒ 
(
ŒŒ 
k
ŒŒ 
=>
ŒŒ 
k
ŒŒ 
.
ŒŒ 

StartsWith
ŒŒ (
(
ŒŒ( )
$"
ŒŒ) +
{
ŒŒ+ ,
	queueName
ŒŒ, 5
}
ŒŒ5 6
_
ŒŒ6 7
"
ŒŒ7 8
)
ŒŒ8 9
)
ŒŒ9 :
.
ŒŒ: ;
ToList
ŒŒ; A
(
ŒŒA B
)
ŒŒB C
;
ŒŒC D
bool
œœ 
result
œœ 
=
œœ 
true
œœ 
;
œœ 
foreach
–– 
(
–– 
var
–– 
queueItemKey
–– %
in
––& (
queueItemKeys
––) 6
)
––6 7
{
—— 
result
““ 
=
““ 
result
““ 
&
““  !
await
““" '
state
““( -
.
““- .!
TryRemoveStateAsync
““. A
(
““A B
queueItemKey
““B N
,
““N O
cancellationToken
““P a
)
““a b
;
““b c
}
”” 
await
‘‘ 
state
‘‘ 
.
‘‘ 
SetIdentity
‘‘ #
(
‘‘# $
	queueName
‘‘$ -
,
‘‘- .
-
‘‘/ 0
$num
‘‘0 1
,
‘‘1 2
cancellationToken
‘‘3 D
)
‘‘D E
;
‘‘E F
await
’’ 
state
’’ 
.
’’ 
SetHeadIdentity
’’ '
(
’’' (
	queueName
’’( 1
,
’’1 2
$num
’’3 4
,
’’4 5
cancellationToken
’’6 G
)
’’G H
;
’’H I
return
÷÷ 
result
÷÷ 
;
÷÷ 
}
◊◊ 	
public
€€ 
static
€€ 
async
€€ 
Task
€€  
<
€€  !
long
€€! %
>
€€% &!
GetQueueLengthAsync
€€' :
(
€€: ;
this
€€; ? 
IActorStateManager
€€@ R
state
€€S X
,
€€X Y
string
€€Z `
	queueName
€€a j
,
€€j k
CancellationToken
‹‹ $
cancellationToken
‹‹% 6
=
‹‹7 8
default
‹‹9 @
(
‹‹@ A
CancellationToken
‹‹A R
)
‹‹R S
)
‹‹S T
{
›› 	
if
ﬁﬁ 
(
ﬁﬁ 
state
ﬁﬁ 
==
ﬁﬁ 
null
ﬁﬁ 
)
ﬁﬁ 
throw
ﬂﬂ 
new
ﬂﬂ $
NullReferenceException
ﬂﬂ 0
(
ﬂﬂ0 1
nameof
ﬂﬂ1 7
(
ﬂﬂ7 8
state
ﬂﬂ8 =
)
ﬂﬂ= >
)
ﬂﬂ> ?
;
ﬂﬂ? @
if
‡‡ 
(
‡‡ 
string
‡‡ 
.
‡‡  
IsNullOrWhiteSpace
‡‡ )
(
‡‡) *
	queueName
‡‡* 3
)
‡‡3 4
)
‡‡4 5
throw
·· 
new
·· 
ArgumentException
·· +
(
··+ ,
nameof
··, 2
(
··2 3
	queueName
··3 <
)
··< =
)
··= >
;
··> ?
long
„„ 
counter
„„ 
=
„„ 
$num
„„ 
;
„„ 
var
‰‰ 
currentIdentity
‰‰ 
=
‰‰  !
await
‰‰" '
state
‰‰( -
.
‰‰- .
GetIdentity
‰‰. 9
(
‰‰9 :
	queueName
‰‰: C
,
‰‰C D
cancellationToken
‰‰E V
)
‰‰V W
;
‰‰W X
var
ÂÂ 
currentHead
ÂÂ 
=
ÂÂ 
await
ÂÂ #
state
ÂÂ$ )
.
ÂÂ) *
GetHeadIdentity
ÂÂ* 9
(
ÂÂ9 :
	queueName
ÂÂ: C
,
ÂÂC D
cancellationToken
ÂÂE V
)
ÂÂV W
;
ÂÂW X
if
ÊÊ 
(
ÊÊ 
(
ÊÊ 
currentIdentity
ÊÊ  
==
ÊÊ! #
-
ÊÊ$ %
$num
ÊÊ% &
&&
ÊÊ' )
currentHead
ÊÊ* 5
==
ÊÊ6 8
$num
ÊÊ9 :
)
ÊÊ: ;
||
ÊÊ< >
currentHead
ÊÊ? J
<=
ÊÊK M
currentIdentity
ÊÊN ]
)
ÊÊ] ^
{
ÁÁ 
counter
ËË 
+=
ËË 
(
ËË 
currentIdentity
ËË +
-
ËË, -
currentHead
ËË. 9
+
ËË: ;
$num
ËË< =
)
ËË= >
;
ËË> ?
}
ÈÈ 
else
ÍÍ 
{
ÎÎ 
counter
ÏÏ 
+=
ÏÏ 
(
ÏÏ 
(
ÏÏ 
long
ÏÏ !
.
ÏÏ! "
MaxValue
ÏÏ" *
-
ÏÏ+ ,
currentHead
ÏÏ- 8
+
ÏÏ9 :
$num
ÏÏ; <
)
ÏÏ< =
+
ÏÏ> ?
(
ÏÏ@ A
currentIdentity
ÏÏA P
+
ÏÏQ R
$num
ÏÏS T
)
ÏÏT U
)
ÏÏU V
;
ÏÏV W
}
ÌÌ 
return
ÓÓ 
counter
ÓÓ 
;
ÓÓ 
}
ÔÔ 	
public
ÛÛ 
static
ÛÛ 
async
ÛÛ 
Task
ÛÛ  
<
ÛÛ  !
TElement
ÛÛ! )
>
ÛÛ) *
PeekQueueAsync
ÛÛ+ 9
<
ÛÛ9 :
TElement
ÛÛ: B
>
ÛÛB C
(
ÛÛC D
this
ÛÛD H 
IActorStateManager
ÛÛI [
state
ÛÛ\ a
,
ÛÛa b
string
ÛÛc i
	queueName
ÛÛj s
,
ÛÛs t
CancellationToken
ÙÙ 
cancellationToken
ÙÙ /
=
ÙÙ0 1
default
ÙÙ2 9
(
ÙÙ9 :
CancellationToken
ÙÙ: K
)
ÙÙK L
)
ÙÙL M
{
ıı 	
if
ˆˆ 
(
ˆˆ 
state
ˆˆ 
==
ˆˆ 
null
ˆˆ 
)
ˆˆ 
throw
˜˜ 
new
˜˜ $
NullReferenceException
˜˜ 0
(
˜˜0 1
nameof
˜˜1 7
(
˜˜7 8
state
˜˜8 =
)
˜˜= >
)
˜˜> ?
;
˜˜? @
if
¯¯ 
(
¯¯ 
string
¯¯ 
.
¯¯  
IsNullOrWhiteSpace
¯¯ )
(
¯¯) *
	queueName
¯¯* 3
)
¯¯3 4
)
¯¯4 5
throw
˘˘ 
new
˘˘ 
ArgumentException
˘˘ +
(
˘˘+ ,
nameof
˘˘, 2
(
˘˘2 3
	queueName
˘˘3 <
)
˘˘< =
)
˘˘= >
;
˘˘> ?
TElement
˚˚ 
result
˚˚ 
=
˚˚ 
default
˚˚ %
(
˚˚% &
TElement
˚˚& .
)
˚˚. /
;
˚˚/ 0
if
˝˝ 
(
˝˝ 
await
˝˝ 
state
˝˝ 
.
˝˝ !
GetQueueLengthAsync
˝˝ /
(
˝˝/ 0
	queueName
˝˝0 9
,
˝˝9 :
cancellationToken
˝˝; L
)
˝˝L M
>
˝˝N O
$num
˝˝P Q
)
˝˝Q R
{
˛˛ 
var
ˇˇ !
currentHeadIdentity
ˇˇ '
=
ˇˇ( )
await
ˇˇ* /
state
ˇˇ0 5
.
ˇˇ5 6
GetHeadIdentity
ˇˇ6 E
(
ˇˇE F
	queueName
ˇˇF O
,
ˇˇO P
cancellationToken
ˇˇQ b
)
ˇˇb c
;
ˇˇc d
var
ÄÄ 
itemKey
ÄÄ 
=
ÄÄ 
GetQueueItemKey
ÄÄ -
(
ÄÄ- .
	queueName
ÄÄ. 7
,
ÄÄ7 8!
currentHeadIdentity
ÄÄ9 L
)
ÄÄL M
;
ÄÄM N
var
ÅÅ 
element
ÅÅ 
=
ÅÅ 
await
ÅÅ #
state
ÅÅ$ )
.
ÅÅ) *
TryGetStateAsync
ÅÅ* :
<
ÅÅ: ;
TElement
ÅÅ; C
>
ÅÅC D
(
ÅÅD E
itemKey
ÅÅE L
,
ÅÅL M
cancellationToken
ÅÅN _
)
ÅÅ_ `
;
ÅÅ` a
if
ÉÉ 
(
ÉÉ 
element
ÉÉ 
.
ÉÉ 
HasValue
ÉÉ $
)
ÉÉ$ %
{
ÑÑ 
result
ÖÖ 
=
ÖÖ 
element
ÖÖ $
.
ÖÖ$ %
Value
ÖÖ% *
;
ÖÖ* +
}
ÜÜ 
}
áá 
return
àà 
result
àà 
;
àà 
}
ââ 	
}
åå 
}çç ±

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
} â
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