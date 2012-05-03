#!/usr/bin/python
#coding:utf-8
"""
	Last Update	: 2011-04-25
	Author		: yqshare
	Site		: http://yqshare.com
	mail		: mail#yqshare.com
	description	: class for fetion
"""
import sys,re,urllib2,urllib,cookielib,os,getpass
import cPickle as p
class fetion(object):
	# '''for users of fetion.

	# send the msg to the users.
	# the member are list:
	# SetInfo 	: ���Ʒ����û���Ϣ
	# LogIn 		����¼����
	# SendWithId 	: ���ݺ���id������Ϣ
	# GetFriendId 	: �ɺ�����Ϣ��ȡ�û�id
	# AddFriend 	�� ��Ӻ���
	# LogOut		�� ע������
	# '''	
	def __init__ (self):
		self.phone=self.pwd=self.loginstatus=self.t=''
		self.operate=''
		self.origURL='http://f.10086.cn/im/login/login.action'
		self.cj=cookielib.LWPCookieJar()
		if os.path.isfile('fetion.coockie'):
			self.cj.revert('fetion.coockie')
		self.opener=urllib2.build_opener( urllib2.HTTPCookieProcessor(self.cj))
		urllib2.install_opener(self.opener)

	def SetInfo(self,phone,password,loginstatus):
		"""
		@phone	     :�ֻ�����
		@password    ����������
		@loginstatus ����¼״̬
			      ����	0
			      ����	1
			      æµ	2
			      �뿪	3
		"""
		self.phone=phone
		self.pwd=password
		self.loginstatus=loginstatus

	def LogIn(self):
		# '''login in the f10086.com
		
		# sucess return 0; fail return -1'''
		params={'m':self.phone,'pass':self.pwd,'loginstatus':self.loginstatus}
		req=urllib2.Request("http://f.10086.cn/im/login/inputpasssubmit1.action",urllib.urlencode(params))
		req.add_header('User-Agent', 'Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_0 like Mac OS X; en-us) \
						AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16')
		self.operate=self.opener.open(req)
		html=self.operate.read()
		#print html
		reg='ontimer=\".*?\"'
		f=re.search(reg,html)
		if f:
			line_content = f.group()
			reg='\d\d\d\d\d*'
			f=re.search(reg,line_content)
			
			if f:
				self.t = f.group()			
				return 0
			else:
				self.t = -1
				return -1
	
	def SendWithId(self,FriendId,Msg):
		# '''send the message to user ...
		# @FriendId �� ����id
		# @Msg �� �����͵���Ϣ
		# '''
		if self.t==-1:
			print '���ȵ�¼...'
			return -1
		params={'msg':Msg,'touchTextLength':'','touchTitle':'','backUrl':''}
		url = "http://f.10086.cn/im/chat/sendMsg.action?touserid="+FriendId
		req=urllib2.Request(url,urllib.urlencode(params))
		req.add_header('User-Agent', 'Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_0 like Mac OS X; en-us) \
						AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16')
		self.operate=self.opener.open(req)
		html=self.operate.read()
		reg='��Ϣ�ɹ�'
		f=re.search(reg,html)
		if f:
			return 0
		else:
			return -1

	def GetFriendId(self,FriendInfo):
		# '''GetFriendId ...
		# @FriendInfo �� �����ֻ�or�����ǳ�
		# '''
		if self.t==-1:
			print '���ȵ�¼...'
			return -1
		params={'searchText':FriendInfo}
		url = "http://f.10086.cn/im/index/searchOtherInfoList.action?t="+self.t
		req=urllib2.Request(url,urllib.urlencode(params))
		req.add_header('User-Agent', 'Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_0 like Mac OS X; en-us) \
						AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16')
		self.operate=self.opener.open(req)
		html=self.operate.read()
		reg='touserid=.*?&'
		f=re.search(reg,html)

		if f:
			line_content = f.group()
			reg='\d\d\d*'
			f=re.search(reg,line_content)
			if f:
				return f.group()		
		return -1


	def AddFriend(self,FriendPhone,NickName):
		# '''��Ӻ��� ...
		# @FriendPhone �� �����ֻ�or���ѷ��ź�
		# @NickName �� ����ǳƻ�����
		# '''
		if self.t==-1:
			print '���ȵ�¼...'
			return -1
		params={'nickname':NickName,'buddylist':0,'localName':'','number':FriendPhone,'type':0}
		url = "http://f.10086.cn/im/user/insertfriendsubmit.action"
		req=urllib2.Request(url,urllib.urlencode(params))
		req.add_header('User-Agent', 'Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_0 like Mac OS X; en-us) \
						AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16')
		self.operate=self.opener.open(req)
		html=self.operate.read()
		reg='���ͳɹ�!���Է�ͬ��'
		f=re.search(reg,html)
		if f:
			return 1
		else:
			return -1
	def LogOut(self):
		# '''�˳� ...
		# '''
		if self.t==-1:
			print '��û�е�¼...'
			return -1
		url = "http://f.10086.cn/im/index/logoutsubmit.action?t="+self.t
		req=urllib2.Request(url)
		req.add_header('User-Agent', 'Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_0 like Mac OS X; en-us) \
						AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16')
		self.operate=self.opener.open(req)



if __name__ == "__main__":
	fetion=fetion()

	phone='15900477006'#raw_input("�����ʺţ�")

	password='Furture67'#getpass.getpass("�������룺")
	#�����¼ --- 1
	loginstatus=1
	#��ʼ���û���Ϣ
	fetion.SetInfo(phone,password,loginstatus)
	#��¼����
	print "��¼�У������ĵȴ�..."
	
	if fetion.LogIn()==-1:
		print "�Բ���,��¼ʧ��..."
		sys.exit()
	 
	print "��ѡ��˵���\n 1.������Ϣ������;\n 2.��Ӻ���;\n 3.�˳���"
	# control = raw_input()
	# if control == '1':
		# while True:
			# print "��������ѵ��ֻ��Ż��߷����ǳƣ�"
			# FriendInfo = raw_input()
			# if FriendInfo=='3':
				# break
		 	# FriendId = fetion.GetFriendId(FriendInfo)
			# if FriendId == -1 :
				# print "���������Ϣ����ȷ�����Ϣ����ĺ��ѣ�"
				# continue
			# print "������Ҫ���͵���Ϣ��"
			# Msg = raw_input()		
			# Statu = fetion.SendWithId(FriendId,Msg)
			# if Statu != -1:
				# print "���ͳɹ�����Ҫ�˳������� 3 ."
			# else:
				# print "����ʧ�ܣ����Ժ�����..."

	# if control == '2':
		# while True:
			# print "������Ҫ��Ӻ��ѵ��ֻ��Ż���źţ�"
			# FriendInfo = raw_input()
			# if FriendInfo=='3':
				# exit
			# print "��������ǳƣ�����Ϣ����ʾ�����ѵ���������ϣ�����"
			# NickName = raw_input()
			# Statu = fetion.AddFriend(FriendInfo,NickName)
			# if Statu != -1 :
				# print "��Ӻ��������ͳɹ�����Ҫ�˳������� 3 ."
			# else:
				# print "��Ӻ���������ʧ�ܣ����Ժ�����..."		

	# if control == '3':
		# exit
	fetion.LogOut()
