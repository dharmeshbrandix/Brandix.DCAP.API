
        public enum eWFDEPStatus
        {
            NA = 0,
            Open = 1,
            Close = 2
        }
         public enum eDEPInstLevel
        {
            NA = 0,
            L1 = 1,
            L2 = 2,
            L3 = 3,
            L4 = 4,
            L5 = 5,
            L6 = 6
        }

        public enum eDEPInstMode
        {
            NA = 0,
            Bulk = 1,
            Counter = 2
        }

        public enum eDEPMulQty
        {
            NA = 0,
            OneQty = 1,
            TwoQty = 2,
            ThreeQty = 3
        }

        public enum eDEPMulQtyCF
        {
            NA = 0,
            Yes = 1,
            NO = 2
        }

        public enum eDEPLimtWithPredecessor
        {
            NA = 0,
            No = 1,
            Yes = 2,
            External = 3,
            PredOpcode = 4,
            BarcodeCheck = 5
        }

        public enum eLimitWithWF
        {
            NA = 0,
            No = 1,
            Yes = 2
        }

        public enum eLimitWithLevel
        {
            NA = 0,
            DEP = 1,
            DEPInst = 2
        }
        public enum eDEPBQSplit
        {
            NA = 0,
            No = 1,
            AutoEqualy = 2,
            AutoRatio = 3,
            Manual = 4
        }

        public enum eCellSelected
        {
            No = 0,
            Yes = 1
        }


        public enum eTxnMode
        {
            Good = 1,
            Scrap = 2
        }
        
        public enum ePlussMinus
        {
            Plus = 1,
            Minus = 2
        }
        public enum eL5BCIsUsed
        {
            No = 1,
            Yes = 2
        }
        public enum eBCCheck
        {
            NA = 0,
            No = 1,
            DEPLevel = 2,
            DEPInstLevel = 3
        }
        public enum eAdjecentNode
        {
            Next = 1,
            Previous = 2
        }

        public enum AppStatus
        {
                        Pending = 1,
            Approved = 2,
            Rejected = 3
        }
        public enum UploadStatus
        {
            Pending = 1,
            Uploaded = 2,
            Rejected = 3
        }
        public enum eL5bcisUsed
        {

            No = 1,
            Yes = 2
        }
        public enum eRecStatus
        {

            Active = 1,
            Inactive = 2
        }
   
     public enum eGetDEPInsDataFor
        {

            Style = 1,
            Shedule = 2,            
            Color = 3,            
            PO = 4,            
            Size = 5

        }

    public enum eOffline
        {
             Online = 0,
             Offline = 1
            
        }

        