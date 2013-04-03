def mean(sorted_list):
    """
    ����������a,b����С��Ϊn,����Ԫ�ص�ֵ����������������
    ��Ҫ��ͨ������a,b�е�Ԫ�أ�ʹ[����aԪ�صĺ�]��[����bԪ�صĺ�]֮��Ĳ���С��
    1. �������кϲ�Ϊһ�����У�������Ϊ����Source
    2. �ó����Ԫ��Big���δ��Ԫ��Small
    3. �����µ�����S[:-2]����ƽ�֣��õ�����max��min
    4. ��Small�ӵ�max���У���Big�Ӵ�min���У����¼��������кͣ��ʹ��Ϊmax��С��Ϊmin��
    """
    if not sorted_list:
        return (([],[]))
    big = sorted_list[-1]
    small = sorted_list[-2]
    big_list, small_list = mean(sorted_list[:-2])
    big_list.append(small)
    small_list.append(big)
    big_list_sum = sum(big_list)
    small_list_sum = sum(small_list)
    return (big_list, small_list) if big_list_sum > small_list_sum else (small_list, big_list)

    
tests = [[1,2,3,4,5,6,700,800],[10001,10000,100,90,50,1],range(1,11),[12312,123,232,210,30,29,3,2,1,1]]
for l in tests:
    l.sort()
    print "Source list:\t",l
    l1,l2 = mean(l)
    print "Result list:\t",l1,l2
    print "Distance:\t",abs(sum(l1)-sum(l2))
