// Copyright (C) 2013 by Autodesk, Inc. All Rights Reserved.
//
// The information contained herein is confidential, proprietary
// to Autodesk, Inc.,  and considered a trade  secret as defined
// in section 499C of the penal code of the State of California.
// Use  of  this  information  by  anyone other  than authorized
// employees of Autodesk, Inc.  is granted  only under a written
// non-disclosure  agreement,  expressly  prescribing  the scope
// and manner of such use.
//
// AUTODESK  MAKES  NO  WARRANTIES,  EXPRESS  OR  IMPLIED, AS  TO THE
// CORRECTNESS OF THIS CODE OR ANY DERIVATIVE WORKS WHICH INCORPORATE
// IT. AUTODESK PROVIDES THE CODE ON AN  "AS-IS" BASIS AND EXPLICITLY
// DISCLAIMS ANY  LIABILITY, INCLUDING  CONSEQUENTIAL AND  INCIDENTAL
// DAMAGES  FOR ERRORS,  OMISSIONS,  AND OTHER  PROBLEMS IN THE CODE.
//
// Use, duplication, or disclosure by the U.S. Government is subject
// to restrictions set forth in  FAR 52.227-19  (Commercial Computer
// Software  Restricted  Rights)  and  DFAR   252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software),  as applicable.
//
#pragma once

#include "ConvertLogger.h"

class ConvertProgess: public FdoBulkCopyProgress
{
public:
    ConvertProgess(long totalCount, const std::wstring& logPath);
    virtual void ObjectAdded(FdoIBulkCopyProgressArgs *args);
    void Dispose();

private:
    long m_TotalNumber;
    ConvertLogger m_progressLogger;
};

