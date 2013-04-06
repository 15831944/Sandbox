#pragma once 
template <typename T>
class Array2D
{
public:
	class Array1D
	{
	public:
		Array1D();// to new array
		Array1D(int count);
		~Array1D();
		Array1D & operator = (const Array1D & rhs);
		T & operator[](int index);
		const T & operator[] (int index) const;
	private:
		T* m_dim2Data;
		int m_dim2;
	};
	Array1D & operator[](int index);
	const Array1D & operator[](int index) const;
	Array2D(int row, int column);
	~Array2D();
private:
	Array1D *  m_dim1Data;
	int m_dim1;
};

template <typename T>
Array2D<T>::Array1D::Array1D()
	: m_dim2(0)
	, m_dim2Data(nullptr)	// necessary to avoid crash when delete in assignment operation
{
}

template <typename T>
Array2D<T>::Array1D::Array1D(int count)
	: m_dim2(count)
{
	m_dim2Data = new T[count];
}

template <typename T>
typename Array2D<T>::Array1D & Array2D<T>::Array1D::operator=(const Array1D & rhs)
{
	if(this == &rhs)
		return *this;
	delete[] m_dim2Data;
	m_dim2 = rhs.m_dim2;
	m_dim2Data = new T[m_dim2];
	for(int i = 0; i < m_dim2; ++i)
		m_dim2Data[i] = rhs.m_dim2Data[i];
	return *this;
}

template <typename T>
Array2D<T>::Array1D::~Array1D()
{
	delete[] m_dim2Data;
}

template <typename T>
T& Array2D<T>::Array1D::operator[](int index)
{
	return m_dim2Data[index];
}

template <typename T>
const T& Array2D<T>::Array1D::operator[](int index) const
{
	return m_dim2Data[index];
}

// Note the typename is necessary for tpye: Array2D<T>::Array1D
// template�ڳ��ֵ��������������ĳ��template��������֮Ϊ�������ơ��������������class�ڳ�Ƕ��״�����֮ΪǶ�״������ơ���Ƕ�״��������п��ܵ��½������ѣ����Ա�������ǰ���Ϲؼ���typename��ʾ����һ�����͡�
template <typename T>
typename Array2D<T>::Array1D & Array2D<T>::operator[](int index)
{
	return m_dim1Data[index];
}

template <typename T>
typename const Array2D<T>::Array1D & Array2D<T>::operator[](int index) const
{
	return m_dim1Data[index];
}

template <typename T>
Array2D<T>::Array2D(int row, int column)
	: m_dim1(row)
{
	// It's not allowed in C++
	// m_dim1Data = new Array1D[row](column);

	m_dim1Data = new Array1D[row];
	for(int i = 0; i < row; ++i)
		m_dim1Data[i] = Array1D(column);
}

template <typename T>
Array2D<T>::~Array2D()
{
	delete[] m_dim1Data;
}