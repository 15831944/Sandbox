#!/usr/bin/python
#coding:utf-8

import ConfigParser
import os
import sys
import string
import cookielib
import urllib
import urllib2
import re

url_login = 'http://f.10086.cn/im/login/inputpasssubmit1.action'
url_logout = 'http://f.10086.cn//im/index/logoutsubmit.action?t='
url_msg = 'http://f.10086.cn/im/user/sendMsgToMyselfs.action'
# user = '15900477006'
# password = 'Furture67'
loginstatus = '4' #����ģʽ
arg_t = ''

class fetion(object):
	def __init__ (self):
		cj = cookielib.LWPCookieJar()
		self.opener = urllib2.build_opener(urllib2.HTTPCookieProcessor(cj))
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
		args = {'pass':self.pwd, 'm':self.phone,'loginstatus':self.loginstatus}
		print 'Logining...'
		req = urllib2.Request(url_login, urllib.urlencode(args))
		jump = self.opener.open(req)
		page = jump.read();
		url = re.compile(r'<card id="start".*?ontimer="(.*?);').findall(page)[0]             
		self.arg_t = re.compile(r't=(\d*)').findall(page)[0]
		if url == '/im/login/login.action':                                                   
			print 'Login Failed!'
			raw_input('Press any key to exit.')
			return False
		else:
			print 'Login Successfully!'
			return True
	
	def SendMe(self, msg):
		if self.arg_t==-1:
			print '���ȵ�¼...'
			return False
		sendmsg = urllib2.Request(url_msg, urllib.urlencode({'msg':msg.decode('gbk').encode('utf-8')}))
		finish = urllib2.urlopen(sendmsg)
		if finish.geturl == 'http://f.10086.cn/im/user/sendMsgToMyself.action' :
			print '������Ϣ���Լ�ʧ��!'
		else:
			print '������Ϣ���Լ��ɹ�'
		
	def GetFriendId(self,FriendInfo):
		# '''GetFriendId ...
		# @FriendInfo �� �����ֻ�or�����ǳ�
		# '''
		if self.arg_t==-1:
			print '���ȵ�¼...'
			return -1
		params = {'searchText':FriendInfo}
		url = "http://f.10086.cn/im/index/searchOtherInfoList.action?t="+self.arg_t
		req = urllib2.Request(url,urllib.urlencode(params))
		jump = self.opener.open(req)
		page = jump.read()
		f=re.search('touserid=.*?&',page)
		if f:
			line_content = f.group()
			f=re.search('\d\d\d*',line_content)
			if f:
				return f.group()	
		print 'δ�ҵ����ֻ��ŵĺ���'
		return -1
		
	def SendWithId(self,FriendId,Msg):
		# print "FriendId:",FriendId
		if self.arg_t==-1:
			print '���ȵ�¼...'
			return False
		params={'msg':msg.decode('gbk').encode('utf-8')}
		url = "http://f.10086.cn/im/chat/sendMsg.action?touserid="+FriendId
		req = urllib2.Request(url,urllib.urlencode(params))
		jump = self.opener.open(req)
		page = jump.read()
		f = re.search('/im/chat/toinputMsg.action?',page)
		if f:
			print '������Ϣ�����ѳɹ�'
			return True
		else:
			print '������Ϣ������ʧ��'
			return False
			
	def LogOut(self):
		if self.arg_t==-1:
			print '��û�е�¼...'
			return False
		url = "http://f.10086.cn/im/index/logoutsubmit.action?t="+self.arg_t
		req = urllib2.Request(url)
		response = self.opener.open(req)
		print 'ע���ɹ�'
		return True

cfg = ConfigParser.ConfigParser()
cfg.read("fetion.ini")
user = cfg.get("Source", "user")
pwd = cfg.get("Source", "pwd")
sendto = cfg.get("Target", "sendto")
msg = cfg.get("Target", "message")	

fetion = fetion()
#�����¼ --- 1
loginstatus=1
fetion.SetInfo(user, pwd, loginstatus)
if fetion.LogIn():
	if user == sendto:
		fetion.SendMe(msg)
	else:
		friendId = fetion.GetFriendId(sendto)
		if -1 != friendId:
			fetion.SendWithId(friendId, msg)
fetion.LogOut()

