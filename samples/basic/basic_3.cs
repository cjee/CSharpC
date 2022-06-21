//EXPECTED:-5

void main()
{
    int input = -5;
    int rez = abs(input);
    Write(rez);
}

int abs(int a)
{
    if( a < 0 )
    {
        return -a;
    }
    else
    {
        return a;
    }
}