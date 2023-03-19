print("****************Lua面向对象***************************")
Object = {}
Object.id = 1;


print("********************封装***************************")
function Object:new()
	--self 是我们默认第一个参数
	--：让第一个参数变成自身

	local obj = {}
	self.__index = self
	setmetatable(obj, self)
	return obj
end

function Object:Test()
	print(self.id)
end

local myObj = Object:new()

print(myObj)
print(myObj.id)
myObj:Test()

--向myObject的表中声明的属性
myObj.id = 2
--所以此时，不会再去元表中寻找属性了
print(myObj.id)
--因为名本身也是一种表里的index，方法是value
--方法被:调用了，方法中的self则变成了“实例化出的对象”本身
--所以现在看起来就像是new了一个Object对象了
myObj:Test()

print("********************继承*****************")
function Object:subClass(className)
	-- _G表 所有的全局变量都存储在当中
	_G[className] = {}

	--通过元表实现继承
	local obj = _G[className]
	self.__index = self
	setmetatable(obj,self)

	--为字类定义base，可以直接访问父类,实现多态
	obj.base = self
end

--person的元表是Object
Object:subClass("Person")
Object:subClass("Monster")

--person的new是Object的new new中的self是Person
--所以p1的元表是Person
local p1= Person:new()
local m1 = Monster:new()
local m2 = Monster:new()
m1.id = 1001

--p1没有id，寻求p1的元表中的__index
--元表Person的__index是Person自身，Person没有id，寻向Person的元表Object中的__index
--Object的__index是它自身，Object有id，返回1
print(p1.id)

--m1有了自己的id，直接输出了m1的ID
--m1与m2互不相干了，就像是创造了两个不同的对象
print(m1.id)
print(m2.id)

print("********************多态*****************")
--相同的行为，不同的表象

Object:subClass("GameObject")
GameObject:subClass("Player")

GameObject.posX = 0
GameObject.posY = 0
function GameObject:Move()
	self.posX = self.posX + 1
	self.posY = self.posY + 1
	print("posX" .. self.posX)
	print("posY" .. self.posY)
end

local player1 = Player:new()
--此时Player没有自身Move方法，所以继承了GameObject的Move方法
player1:Move()

function Player:Move()
	-- 天坑
	-- base是GameObject（表）
	-- 此时这样进行冒号调用，方法内的self是GameObject不是Player
	-- self.base:Move()
	-- 如果需要执行父类逻辑，需要使用.调用，再把自己传进入
	self.base.Move(self)

	--self.posX = self.posX + 5
	--print("posX" .. self.posX)
end
--此时Player的Move方法override了GameObject的Move
--可以调用自己所属的Move了
player1:Move()

--P1和P2分开了
local player2 = Player:new()
player2:Move()
player1:Move()



